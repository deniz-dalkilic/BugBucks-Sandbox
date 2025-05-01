using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace OrderService.Api.Authorization;

public class OwnerOrAdminHandler : AuthorizationHandler<OwnerOrAdminRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OwnerOrAdminRequirement requirement)
    {
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (context.Resource is HttpContext httpContext)
            if (httpContext.Request.RouteValues.TryGetValue("userId", out var routeUser)
                && routeUser?.ToString() == userId)
                context.Succeed(requirement);

        return Task.CompletedTask;
    }
}