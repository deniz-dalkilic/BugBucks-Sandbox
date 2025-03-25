using IdentityService.Api.Models;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IApplicationUserRepository _userRepository;

    public AuthController(ITokenService tokenService, IApplicationUserRepository userRepository)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserNameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("UserNameOrEmail and Password are required.");

        ApplicationUser? user = null;

        // Check if input is an email address based on the presence of '@'
        if (request.UserNameOrEmail.Contains("@"))
            // Search by email
            user = await _userRepository.GetUserByEmailAsync(request.UserNameOrEmail);
        else
            // Search by username
            user = await _userRepository.GetUserByUserNameAsync(request.UserNameOrEmail);

        if (user == null)
            return Unauthorized("User not found.");

        // For demonstration only: compare passwords directly (in production, use proper hashing/UserManager)
        if (user.PasswordHash != request.Password)
            return Unauthorized("Invalid credentials.");

        var token = await _tokenService.GenerateTokenAsync(user);
        return Ok(new { token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Username, Email and Password are required.");

        // Check if a user already exists by email or username
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email)
                           ?? await _userRepository.GetUserByUserNameAsync(request.UserName);

        if (existingUser != null)
            return BadRequest("User with given email or username already exists.");

        var newUser = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            // For demo purposes, setting the PasswordHash directly (in production, use proper hashing via UserManager)
            PasswordHash = request.Password
        };

        var createdUser = await _userRepository.CreateUserAsync(newUser);
        return CreatedAtAction(nameof(UsersController.GetUser), "Users", new { id = createdUser.Id }, createdUser);
    }
}