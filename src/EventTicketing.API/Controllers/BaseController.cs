using Microsoft.AspNetCore.Mvc;
using MediatR;
using ErrorOr;
using AutoMapper;

namespace EventTicketing.API.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    private ISender _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    
    private readonly IMapper _mapper;
    protected IMapper Mapper => _mapper;
    

    protected IActionResult HandleResult<T>(ErrorOr<T> result)
    {
        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        return Ok(result.Value);
    }
    
    protected IActionResult Problem(List<Error> errors)
    {
        if (errors.All(e => e.Type == ErrorType.Validation))
        {
            var problemDetails = new ValidationProblemDetails
            {
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "See the errors property for details.",
            };

            foreach (var error in errors)
            {
                problemDetails.Errors.Add(error.Code, new[] { error.Description });
            }

            return BadRequest(problemDetails);
        }

        var firstError = errors[0];

        var statusCode = firstError.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var genericProblemDetails = new ProblemDetails
        {
            Title = firstError.Description,
            Status = statusCode,
            Detail = firstError.Description
        };

        return StatusCode(statusCode, genericProblemDetails);
    }

}