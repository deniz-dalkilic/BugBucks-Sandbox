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

    // GET: api/users/{externalId}
    // Only the resource owner or an admin can access this endpoint.
    [HttpGet("{externalId:guid}")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> GetUser(Guid externalId)
    {
        var user = await _userRepository.GetUserByExternalIdAsync(externalId);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    // DELETE: api/users/{externalId}
    // Soft-delete: marks the user as deleted.
    [HttpDelete("{externalId:guid}")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> DeleteUser(Guid externalId)
    {
        var user = await _userRepository.GetUserByExternalIdAsync(externalId);
        if (user == null)
            return NotFound();

        await _userRepository.SoftDeleteUserAsync(user);
        return NoContent();
    }
}