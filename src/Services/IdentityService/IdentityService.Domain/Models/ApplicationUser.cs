using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain.Models;

// Using IdentityUser<int> for internal primary key (int)
public class ApplicationUser : IdentityUser<int>
{
    // External identifier for API usage
    public Guid ExternalId { get; set; } = Guid.NewGuid();

    // Soft-delete flag
    public bool IsDeleted { get; set; } = false;

    // Audit fields
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }

    // Additional properties
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}