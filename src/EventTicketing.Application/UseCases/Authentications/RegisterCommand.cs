using ErrorOr;
using MediatR;
using EventTicketing.Application.Models;
using EventTicketing.Domain.Entities;
namespace EventTicketing.Application.UseCases.Authentications;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password
    ) : IRequest<ErrorOr<AuthenticationResult>>;