using BugBucks.Shared.Logging.Interfaces;
using IdentityService.Api.Models;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAppLogger<AuthController> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IAppLogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            ExternalId = Guid.NewGuid(),
            CreatedDate = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        // Assign default role "User"
        await _userManager.AddToRoleAsync(user, "User");

        return CreatedAtAction(nameof(GetUser), new { externalId = user.ExternalId },
            new { user.ExternalId, user.UserName, user.Email });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.UserNameOrEmail) || string.IsNullOrEmpty(request.Password))
            return BadRequest("UserNameOrEmail and Password are required.");


        ApplicationUser user = null;
        if (request.UserNameOrEmail.Contains("@"))
            user = await _userManager.FindByEmailAsync(request.UserNameOrEmail);
        else
            user = await _userManager.FindByNameAsync(request.UserNameOrEmail);

        if (user == null) return Unauthorized("User not found.");

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!signInResult.Succeeded) return Unauthorized("Invalid credentials.");

        // Retrieve user roles from UserManager
        var roles = await _userManager.GetRolesAsync(user);
        var token = await _tokenService.GenerateTokenAsync(user, roles);
        return Ok(new { token });
    }

    [HttpGet("user/{externalId:guid}")]
    public async Task<IActionResult> GetUser(Guid externalId)
    {
        // Retrieve the currently authenticated user's ExternalId from claims
        var currentUserExternalIdClaim = User.FindFirst("ExternalId")?.Value;
        if (!Guid.TryParse(currentUserExternalIdClaim, out var currentUserExternalId)) return Unauthorized();

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.ExternalId == externalId && !u.IsDeleted);
        if (user == null)
            return NotFound();

        // If the user is not an Admin, they can only access their own data
        if (!User.IsInRole("Admin") && currentUserExternalId != externalId) return Forbid();

        return Ok(user);
    }
}