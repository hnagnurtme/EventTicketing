using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EventTicketing.Contracts.Authentication;
using EventTicketing.Application.Services.Authentication.Commands.Register;
using EventTicketing.Application.Services.Authentication.Common;

using MapsterMapper;


namespace EventTicketing.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : BaseController
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
        return HandleResult(authResult);
    }
}