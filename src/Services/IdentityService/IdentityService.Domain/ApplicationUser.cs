using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain;

/// <summary>
///     Custom user entity extending IdentityUser with additional properties.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    ///     Gets or sets the full name of the user.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    ///     Gets or sets the date of birth of the user.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }
}