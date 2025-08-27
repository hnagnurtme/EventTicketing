namespace EventTicketing.Contracts.Authentication;
public record AuthenticationResponse(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string Token
);