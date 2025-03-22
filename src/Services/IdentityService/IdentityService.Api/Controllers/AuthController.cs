using System.Security.Claims;
using BugBucks.Shared.Logging;
using IdentityService.Api.Models;
using IdentityService.Domain.Models;
using IdentityService.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        _logger.LogInformation("Register endpoint called for user {UserName}", model.UserName);

        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email,
            FullName = model.FullName,
            DateOfBirth = model.DateOfBirth
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            _logger.LogError(null, "User registration failed for {UserName}", model.UserName);
            return BadRequest(result.Errors);
        }

        _logger.LogInformation("User {UserName} registered successfully", model.UserName);
        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        _logger.LogInformation("Login attempt for user {UserName}", model.UserName);

        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user == null)
        {
            _logger.LogError(null, "User {UserName} not found", model.UserName);
            return Unauthorized("Invalid credentials.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
        {
            _logger.LogError(null, "Invalid credentials for user {UserName}", model.UserName);
            return Unauthorized("Invalid credentials.");
        }

        var token = _tokenService.GenerateToken(user);
        _logger.LogInformation("User {UserName} logged in successfully", model.UserName);
        return Ok(new { Token = token });
    }

    [HttpGet("external-login")]
    public IActionResult ExternalLogin(string provider, string returnUrl = null)
    {
        _logger.LogInformation("External login initiated for provider {Provider}", provider);
        var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { ReturnUrl = returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet("external-login-callback")]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        if (!string.IsNullOrEmpty(remoteError))
        {
            _logger.LogError(null, "Error from external provider: {Error}", remoteError);
            return RedirectToAction("Login");
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            _logger.LogError(null, "Failed to load external login information.");
            return RedirectToAction("Login");
        }

        // Sign in user with external provider
        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in with {Provider} provider.", info.LoginProvider);
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            var token = _tokenService.GenerateToken(user);
            return Ok(new { Token = token });
        }
        else
        {
            // If user doesn't exist, create and sign in the user
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var userName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email;
            var user = new ApplicationUser { UserName = userName, Email = email };

            var createResult = await _userManager.CreateAsync(user);
            if (createResult.Succeeded)
            {
                createResult = await _userManager.AddLoginAsync(user, info);
                if (createResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    _logger.LogInformation("User created and logged in with {Provider} provider.", info.LoginProvider);
                    var token = _tokenService.GenerateToken(user);
                    return Ok(new { Token = token });
                }
            }

            _logger.LogError(null, "External login failed.");
            return BadRequest("External login failed.");
        }
    }
}