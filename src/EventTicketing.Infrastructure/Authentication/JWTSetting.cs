namespace EventTicketing.Infrastructure.Authentication;

public class JWTSettings
{
    public const string SectionName = "JwtSettings";
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = "super-secret-key-that-is-long-enough";

    public int ExpirationMinutes { get; set; } = 30;
}