using EventTicketing.Domain.Entities;
using EventTicketing.Application.Common.Interface.Authentication;

using ErrorOr;
using MediatR;
using EventTicketing.Application.Common.Interface.Persistence;
using EventTicketing.Application.DTOs.Authentication;

namespace EventTicketing.Application.Services.Authentication.Commands.Register;
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<AuthenticationResult>>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    private readonly IUserRepository _userRepository;

    public RegisterCommandHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
    }
    public async Task<ErrorOr<AuthenticationResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        /*
            1. Check if the user already exists
            2. Hash the password
            3. Save the user to the database
            4. Generate a JWT token
            5. Return the AuthenciationResult with user details and token
        */
        if (_userRepository.GetUserByEmail(command.Email) != null)
        {
            return Error.Failure("DuplicateEmail", "Email already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            Password = command.Password, 
        };

        _userRepository.AddUser(user);

        var token = _jwtTokenGenerator.GenerateToken(
            user.Id.ToString(),
            user.FirstName,
            user.LastName
        );

        return new AuthenticationResult(
            user,
            token
        );
    }
}