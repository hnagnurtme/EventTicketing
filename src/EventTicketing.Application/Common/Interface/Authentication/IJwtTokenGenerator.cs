namespace EventTicketing.Application.Common.Interface.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string firstName, string lastName);
}
