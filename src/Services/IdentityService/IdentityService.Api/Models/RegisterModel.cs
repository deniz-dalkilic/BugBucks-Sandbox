namespace IdentityService.Api.Models;

/// <summary>
///     Represents a user registration request.
/// </summary>
public class RegisterModel
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public DateTime? DateOfBirth { get; set; }
}