using Microsoft.AspNetCore.Mvc;

namespace EventTicketing.API.Controllers;

[ApiController]
[Route("api/events")]
public class EventController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Event endpoint is working!");
    }

    [HttpPost]
    public IActionResult CreateEvent()
    {
        return Ok("Event created successfully!");
    }
}