namespace EventTicketing.Application.Models;
using EventTicketing.Domain.Entities;
public record AuthenticationResult(
    User User,
    string Token
);