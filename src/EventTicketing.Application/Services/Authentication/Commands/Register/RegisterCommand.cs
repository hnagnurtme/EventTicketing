using MediatR;
using ErrorOr;
using EventTicketing.Application.DTOs.Authentication;

namespace EventTicketing.Application.Services.Authentication.Commands.Register;

using ErrorOr;
using MediatR;
using EventTicketing.Application.DTOs.Authentication;


public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : IRequest<ErrorOr<AuthenticationResult>>;