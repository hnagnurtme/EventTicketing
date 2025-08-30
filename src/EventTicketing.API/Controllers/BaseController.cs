using Microsoft.AspNetCore.Mvc;
using EventTicketing.API.Common;
using EventTicketing.Application.Common.Models;
using ErrorOr;

namespace EventTicketing.API.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResult<T>(ErrorOr<T> result, string? successMessage = null)
    {
        if (result.IsError)
        {
            return HandleError<T>(result.Errors);
        }

        var value = result.Value;

        if (value == null)
            return NoContent();

        return Ok(ApiResponse<T>.Ok(value, successMessage));
    }

    private IActionResult HandleError<T>(List<Error> errors)
    {
        var firstError = errors[0];

        var statusCode = firstError.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var response = new ApiResponse<T>
        {
            Success = false,
            Message = firstError.Description,
            StatusCode = statusCode,
            Data = default
        };

        return StatusCode(statusCode, response);
    }

    protected IActionResult HandlePagedResult<T>(ErrorOr<PagedResult<T>> result, string? successMessage = null)
    {
        if (result.IsError)
            return HandleError<PagedResult<T>>(result.Errors);

        var paged = result.Value;
        if (paged == null || paged.Items == null || !paged.Items.Any())
            return NoContent();

        var meta = new
        {
            paged.PageNumber,
            paged.PageSize,
            paged.TotalItems,
            paged.TotalPages
        };

        return Ok(ApiResponse<PagedResult<T>>.Ok(paged, successMessage, meta));
    }
}
