using IdentityService.Domain.Models;

namespace IdentityService.Application.Interfaces;

public interface IApplicationUserService
{
    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
    Task<ApplicationUser?> GetUserByExternalIdAsync(Guid externalId);
    Task<ApplicationUser> CreateUserAsync(ApplicationUser user);
    Task UpdateUserAsync(ApplicationUser user);
    Task SoftDeleteUserAsync(Guid externalId);
}