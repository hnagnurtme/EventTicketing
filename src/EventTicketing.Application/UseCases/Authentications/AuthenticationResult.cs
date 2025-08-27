namespace EventTicketing.Application.Models;
public record AuthenticationResult(
    User User,
    string Token
);