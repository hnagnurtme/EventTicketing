using FluentValidation;
using EventTicketing.Application.Services.Authentication.Commands.Register;

namespace EventTicketing.Application.Services.Authentication.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required");
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required");
    }
}