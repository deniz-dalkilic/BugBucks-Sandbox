using IdentityService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IApplicationUserRepository _userRepository;

    public UsersController(IApplicationUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    // GET: api/users/{externalId}
    [HttpGet("{externalId:guid}")]
    public async Task<IActionResult> GetUser(Guid externalId)
    {
        var user = await _userRepository.GetUserByExternalIdAsync(externalId);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    // DELETE: api/users/{id}
    // Soft-delete: mark the user as deleted
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
        await _userRepository.SoftDeleteUserAsync(user);
        return NoContent();
    }
}