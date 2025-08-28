using MediatR;
using ErrorOr;
using EventTicketing.Application.Services.Authentication.Common;

namespace EventTicketing.Application.Services.Authentication.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password
) : IRequest<ErrorOr<AuthenticationResult>>;