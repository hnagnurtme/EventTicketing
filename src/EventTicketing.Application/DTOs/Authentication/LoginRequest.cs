namespace EventTicketing.Application.DTOs.Authentication;

public record LoginRequest(
    string Email,
    string Password
);
