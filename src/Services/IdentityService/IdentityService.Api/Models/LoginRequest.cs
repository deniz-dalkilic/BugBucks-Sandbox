using System.ComponentModel.DataAnnotations;

namespace IdentityService.Api.Models;

public class LoginRequest
{
    [Required] public string UserNameOrEmail { get; set; } = string.Empty;

    [Required] public string Password { get; set; } = string.Empty;
}