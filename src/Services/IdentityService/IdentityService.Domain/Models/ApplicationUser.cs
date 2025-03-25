using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain.Models;

// Use IdentityUser<int> for internal primary key as integer
public class ApplicationUser : IdentityUser<int>
{
    // External identifier used for API calls
    public Guid ExternalId { get; set; } = Guid.NewGuid();

    // Soft delete flag
    public bool IsDeleted { get; set; } = false;

    // Additional properties
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}