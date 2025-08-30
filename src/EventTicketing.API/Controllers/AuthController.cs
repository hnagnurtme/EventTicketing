using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EventTicketing.Application.DTOs.Authentication;
using EventTicketing.Application.Services.Authentication.Commands.Register;
using EventTicketing.API.Common.Helper;
using MapsterMapper;


namespace EventTicketing.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public AuthController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = _mapper.Map<RegisterCommand>(request);
        ErrorOr<AuthenticationResult> authResult = await _mediator.Send(command);
        return CREATE.HandleResult(authResult, "User registered successfully");
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        await Task.Delay(0);
        return Ok();
    }
}