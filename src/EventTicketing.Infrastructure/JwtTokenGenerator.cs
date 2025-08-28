using EventTicketing.Shared;
using EventTicketing.Application.Services.Authentication.Commands.Register;
using Microsoft.Extensions.Options;

namespace EventTicketing.Infrastructure;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenGenerator(Microsoft.Extensions.Options.IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateToken(string userId, string firstName, string lastName)
    {
        // TODO: Implement JWT token generation logic using _jwtSettings
        // For now, return a dummy token
        return $"dummy-token-for-{userId}";
    }
}
