using Microsoft.AspNetCore.Mvc;

namespace BugBucks.BuildingBlocks.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected ActionResult<T> OkOrNotFound<T>(T? data) where T : class
    {
        return data == null ? NotFound() : Ok(data);
    }

    protected ActionResult Fail(string message)
    {
        return BadRequest(new { error = message });
    }
}