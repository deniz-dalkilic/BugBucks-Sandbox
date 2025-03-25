using IdentityService.Domain.Models;

namespace IdentityService.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(ApplicationUser user);
}