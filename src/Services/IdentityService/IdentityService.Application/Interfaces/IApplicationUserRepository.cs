using IdentityService.Domain.Models;

namespace IdentityService.Application.Interfaces;

public interface IApplicationUserRepository
{
    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
    Task<ApplicationUser?> GetUserByIdAsync(int id);
    Task<ApplicationUser?> GetUserByExternalIdAsync(Guid externalId);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<ApplicationUser?> GetUserByUserNameAsync(string userName);
    Task<ApplicationUser> CreateUserAsync(ApplicationUser user);
    Task UpdateUserAsync(ApplicationUser user);
    Task SoftDeleteUserAsync(ApplicationUser user);
}