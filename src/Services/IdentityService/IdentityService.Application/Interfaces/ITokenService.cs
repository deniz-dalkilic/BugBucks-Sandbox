using IdentityService.Domain.Models;

namespace IdentityService.Application.Interfaces;

public interface ITokenService
{
    // Updated to include roles parameter
    Task<string> GenerateTokenAsync(ApplicationUser user, IList<string> roles);
}