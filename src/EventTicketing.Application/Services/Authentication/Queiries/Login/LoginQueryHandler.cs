
using EventTicketing.Application.DTOs.Authentication;
using EventTicketing.Domain.Entities;
using ErrorOr;
using MediatR;
using EventTicketing.Application.Common.Interface.Authentication;
using EventTicketing.Application.Common.Interface.Persistence;

namespace EventTicketing.Application.Services.Authentication.Queiries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, ErrorOr<AuthenticationResult>>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    private readonly IUserRepository _userRepository;

    public LoginQueryHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<AuthenticationResult>> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        /*
            1. Validate the email and password
            2. Check if the user exists in database
            3. Generate JWT token
            4. Return the AuthenciationResult with user details and token
        */
        if (_userRepository.GetUserByEmail(query.Email) is not User user)
        {
            return Error.Failure("InvalidCredential", "Email or password is incorrect");
        }

        if (user.Password != query.Password)
        {
            return Error.Failure("InvalidCredential", "Email or password is incorrect");
        }

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