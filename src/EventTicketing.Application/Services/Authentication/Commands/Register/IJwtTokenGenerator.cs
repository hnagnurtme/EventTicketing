namespace EventTicketing.Application.Services.Authentication.Commands.Register;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string firstName, string lastName);
}
