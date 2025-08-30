using EventTicketing.API.Middlewares;
using Serilog;



namespace EventTicketing.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder ConfigureEventTicketingPipeline(this IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseErrorHandling();
            app.UseSerilogRequestLogging(opts =>
            {
                opts.GetLevel = (httpContext, elapsed, ex) =>
                {
                    if (ex != null)
                    {
                        return Serilog.Events.LogEventLevel.Error;
                    }

                    if (httpContext.Response.StatusCode >= 500)
                    {

                        return Serilog.Events.LogEventLevel.Warning;
                    }

                    return Serilog.Events.LogEventLevel.Information;
                };
            });

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("/swagger");
                    return Task.CompletedTask;
                });
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            return app;
        }
    }
}