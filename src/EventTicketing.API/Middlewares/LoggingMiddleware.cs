using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace EventTicketing.API.Middlewares
{
    public class RequestLog
    {
    public string? TraceId { get; set; } = null;
    public string? Method { get; set; } = null;
    public string? Path { get; set; } = null;
    public string? QueryString { get; set; } = null;
    public string? UserAgent { get; set; } = null;
    public string? RemoteIpAddress { get; set; } = null;
    public string? RequestBody { get; set; } = null;
    public DateTime RequestTime { get; set; }
    public string? UserId { get; set; } = null;
    public string? Headers { get; set; } = null;
    }
    
    public class ResponseLog
    {
        public string? TraceId { get; set; } = null;
        public int StatusCode { get; set; }
        public string? ResponseBody { get; set; } = null;
        public long ResponseTimeMs { get; set; }
        public DateTime ResponseTime { get; set; }
        public long ResponseSizeBytes { get; set; }
    }

    public class RequestResponseLog
    {
    public RequestLog? Request { get; set; } = null;
    public ResponseLog? Response { get; set; } = null;
    public long TotalTimeMs { get; set; }
    public bool IsSuccessful => Response?.StatusCode >= 200 && Response?.StatusCode < 400;
    }

    public class LoggingMiddlewareOptions
    {
        public bool LogRequestBody { get; set; } = true;
        public bool LogResponseBody { get; set; } = true;
        public bool LogHeaders { get; set; } = true;
        public int MaxBodyLength { get; set; } = 4096;
        public string[] ExcludePaths { get; set; } = { "/health", "/metrics", "/favicon.ico" };
        public string[] SensitiveHeaders { get; set; } = { "authorization", "cookie", "x-api-key" };
        public bool LogOnlyErrors { get; set; } = false;
    }

    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;
        private readonly LoggingMiddlewareOptions _options;

        public LoggingMiddleware(
            RequestDelegate next, 
            ILogger<LoggingMiddleware> logger, 
            LoggingMiddlewareOptions? options = null)
        {
            _next = next;
            _logger = logger;
            _options = options ?? new LoggingMiddlewareOptions();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (ShouldSkipLogging(context.Request.Path))
            {
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            var requestLog = await LogRequestAsync(context);

            var originalResponseBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
                stopwatch.Stop();

                var responseLog = await LogResponseAsync(context, responseBody, stopwatch.ElapsedMilliseconds);
                
                LogRequestResponse(requestLog, responseLog, stopwatch.ElapsedMilliseconds);
                await responseBody.CopyToAsync(originalResponseBodyStream);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                var errorResponseLog = new ResponseLog
                {
                    TraceId = context.TraceIdentifier,
                    StatusCode = 500,
                    ResponseBody = "Internal Server Error",
                    ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                    ResponseTime = DateTime.UtcNow
                };

                LogRequestResponse(requestLog, errorResponseLog, stopwatch.ElapsedMilliseconds);
                
                _logger.LogError(ex, "Exception occurred during request processing. TraceId: {TraceId}", 
                    context.TraceIdentifier);

                context.Response.Body = originalResponseBodyStream;
                throw;
            }
            finally
            {
                context.Response.Body = originalResponseBodyStream;
            }
        }

        private bool ShouldSkipLogging(string path)
        {
            foreach (var excludePath in _options.ExcludePaths)
            {
                if (path.StartsWith(excludePath, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private async Task<RequestLog> LogRequestAsync(HttpContext context)
        {
            var request = context.Request;
            
            var requestLog = new RequestLog
            {
                TraceId = context.TraceIdentifier,
                Method = request.Method,
                Path = request.Path,
                QueryString = request.QueryString.ToString(),
                UserAgent = request.Headers["User-Agent"].ToString(),
                RemoteIpAddress = GetClientIpAddress(context),
                RequestTime = DateTime.UtcNow,
                UserId = GetUserId(context)
            };

            if (_options.LogHeaders)
            {
                requestLog.Headers = GetSafeHeaders(request.Headers);
            }
            if (_options.LogRequestBody && HasBody(request))
            {
                request.EnableBuffering();
                var body = await ReadStreamAsync(request.Body);
                requestLog.RequestBody = TruncateString(body, _options.MaxBodyLength);
                request.Body.Position = 0;
            }

            return requestLog;
        }

        private async Task<ResponseLog> LogResponseAsync(HttpContext context, MemoryStream responseBody, long elapsedMs)
        {
            var responseLog = new ResponseLog
            {
                TraceId = context.TraceIdentifier,
                StatusCode = context.Response.StatusCode,
                ResponseTimeMs = elapsedMs,
                ResponseTime = DateTime.UtcNow,
                ResponseSizeBytes = responseBody.Length
            };
            if (_options.LogResponseBody && responseBody.Length > 0)
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                var body = await ReadStreamAsync(responseBody);
                responseLog.ResponseBody = TruncateString(body, _options.MaxBodyLength);
                responseBody.Seek(0, SeekOrigin.Begin);
            }

            return responseLog;
        }

        private void LogRequestResponse(RequestLog requestLog, ResponseLog responseLog, long totalTimeMs)
        {
            var logData = new RequestResponseLog
            {
                Request = requestLog,
                Response = responseLog,
                TotalTimeMs = totalTimeMs
            };
            if (_options.LogOnlyErrors && logData.IsSuccessful)
                return;

            var logLevel = DetermineLogLevel(responseLog.StatusCode);
            var message = "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms";

            var logArgs = new object[]
            {
                requestLog.Method ?? string.Empty,
                requestLog.Path ?? string.Empty,
                responseLog.StatusCode,
                totalTimeMs
            };
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["TraceId"] = requestLog.TraceId ?? string.Empty,
                ["UserId"] = requestLog.UserId ?? string.Empty,
                ["RemoteIp"] = requestLog.RemoteIpAddress ?? string.Empty,
                ["UserAgent"] = requestLog.UserAgent ?? string.Empty,
                ["RequestBody"] = requestLog.RequestBody ?? string.Empty,
                ["ResponseBody"] = responseLog.ResponseBody ?? string.Empty,
                ["ResponseSize"] = responseLog.ResponseSizeBytes,
                ["Headers"] = requestLog.Headers ?? string.Empty
            }))
            {
                _logger.Log(logLevel, message, logArgs);
            }

            // Performance warning cho slow requests
            if (totalTimeMs > 5000) // > 5 seconds
            {
                _logger.LogWarning("Slow request detected: {Method} {Path} took {ElapsedMs}ms. TraceId: {TraceId}",
                    requestLog.Method, requestLog.Path, totalTimeMs, requestLog.TraceId);
            }
        }

        private static LogLevel DetermineLogLevel(int statusCode)
        {
            return statusCode switch
            {
                >= 500 => LogLevel.Error,
                >= 400 => LogLevel.Warning,
                >= 300 => LogLevel.Information,
                _ => LogLevel.Information
            };
        }

        private static string GetClientIpAddress(HttpContext context)
        {
            var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(ipAddress))
            {
                return ipAddress.Split(',')[0].Trim();
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private static string GetUserId(HttpContext context)
        {
            return context.User?.Identity?.Name ?? 
                   context.User?.FindFirst("sub")?.Value ?? 
                   context.User?.FindFirst("id")?.Value ?? 
                   "Anonymous";
        }

        private string GetSafeHeaders(IHeaderDictionary headers)
        {
            var safeHeaders = new Dictionary<string, string>();
            
            foreach (var header in headers)
            {
                var isSensitive = _options.SensitiveHeaders.Any(sh => 
                    string.Equals(sh, header.Key, StringComparison.OrdinalIgnoreCase));
                
                safeHeaders[header.Key] = isSensitive ? "***MASKED***" : header.Value.ToString();
            }

            return JsonSerializer.Serialize(safeHeaders);
        }

        private static bool HasBody(HttpRequest request)
        {
            return request.ContentLength > 0 || 
                   string.Equals(request.Method, "POST", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(request.Method, "PUT", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(request.Method, "PATCH", StringComparison.OrdinalIgnoreCase);
        }

        private static async Task<string> ReadStreamAsync(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            return await reader.ReadToEndAsync();
        }

        private static string TruncateString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
                return value;

            return value.Substring(0, maxLength) + "... [TRUNCATED]";
        }
    }

    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(
            this IApplicationBuilder builder, 
            LoggingMiddlewareOptions? options = null)
        {
            return builder.UseMiddleware<LoggingMiddleware>(options ?? new LoggingMiddlewareOptions());
        }

        public static IApplicationBuilder UseRequestResponseLogging(
            this IApplicationBuilder builder, 
            Action<LoggingMiddlewareOptions> configureOptions)
        {
            var options = new LoggingMiddlewareOptions();
            configureOptions(options);
            return builder.UseMiddleware<LoggingMiddleware>(options);
        }
    }
}

