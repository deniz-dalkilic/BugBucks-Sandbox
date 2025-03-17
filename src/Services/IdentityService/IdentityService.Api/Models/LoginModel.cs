namespace IdentityService.Api.Models;

/// <summary>
///     Represents a user login request.
/// </summary>
public class LoginModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
}