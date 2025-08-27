using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EventTicketing.Contracts.Authentication;
namespace EventTicketing.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : BaseController
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        return Ok("Login endpoint is working!");
    }


    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        return Ok("Register endpoint is working!");
    }
}