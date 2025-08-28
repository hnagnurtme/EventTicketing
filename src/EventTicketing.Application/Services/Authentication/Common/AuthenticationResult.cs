
using EventTicketing.Domain.Entities;

namespace EventTicketing.Application.Services.Authentication.Common;

public record AuthenticationResult(
    User User,
    string Token
);