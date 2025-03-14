using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("Identity Service is up and running!");
    }
}