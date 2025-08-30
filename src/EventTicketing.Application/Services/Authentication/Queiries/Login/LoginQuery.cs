using EventTicketing.Application.DTOs.Authentication;
using ErrorOr;
using MediatR;

namespace EventTicketing.Application.Services.Authentication.Queiries.Login;

public record LoginQuery(
    string Email,
    string Password
) : IRequest<ErrorOr<AuthenticationResult>>;