using System.ComponentModel.DataAnnotations;

namespace IdentityService.Api.Models;

public class RegisterRequest
{
    [Required] public string UserName { get; set; } = string.Empty;

    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;

    [Required] public string Password { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }
}