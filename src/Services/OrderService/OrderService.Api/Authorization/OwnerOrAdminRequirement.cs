using Microsoft.AspNetCore.Authorization;

namespace OrderService.Api.Authorization;

public class OwnerOrAdminRequirement : IAuthorizationRequirement
{
}