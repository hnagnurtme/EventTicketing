using EventTicketing.Domain.Entities;

namespace EventTicketing.Application.DTOs.Authentication;

public class AuthenticationResult
{
    public  User User { get; set; }
    public  string Token { get; set; }

    public AuthenticationResult(User user, string token)
    {
        User = user;
        Token = token;
    }
}