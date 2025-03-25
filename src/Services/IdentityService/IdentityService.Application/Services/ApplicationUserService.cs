using IdentityService.Application.Interfaces;
using IdentityService.Domain.Models;

namespace IdentityService.Application.Services;

public class ApplicationUserService : IApplicationUserService
{
    private readonly IApplicationUserRepository _userRepository;

    public ApplicationUserService(IApplicationUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUsersAsync();
    }

    public async Task<ApplicationUser?> GetUserByExternalIdAsync(Guid externalId)
    {
        return await _userRepository.GetUserByExternalIdAsync(externalId);
    }

    public async Task<ApplicationUser> CreateUserAsync(ApplicationUser user)
    {
        if (user.ExternalId == Guid.Empty) user.ExternalId = Guid.NewGuid();
        return await _userRepository.CreateUserAsync(user);
    }

    public async Task UpdateUserAsync(ApplicationUser user)
    {
        await _userRepository.UpdateUserAsync(user);
    }

    public async Task SoftDeleteUserAsync(Guid externalId)
    {
        var user = await _userRepository.GetUserByExternalIdAsync(externalId);
        if (user != null)
        {
            user.IsDeleted = true;
            await _userRepository.UpdateUserAsync(user);
        }
    }
}