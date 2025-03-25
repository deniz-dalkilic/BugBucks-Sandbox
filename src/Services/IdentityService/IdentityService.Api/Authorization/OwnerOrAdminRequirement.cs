using Microsoft.AspNetCore.Authorization;

namespace IdentityService.Api.Authorization;

public class OwnerOrAdminRequirement : IAuthorizationRequirement
{
}