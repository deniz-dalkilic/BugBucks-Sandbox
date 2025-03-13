using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace LoggingService.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DemoController : ControllerBase
{
    [HttpGet("test-log")]
    public IActionResult TestLog()
    {
        Log.Information("This is a test log from LoggingService!");
        return Ok("Log has been recorded. Check the console or configured sink.");
    }
}