using Microsoft.AspNetCore.Authorization;

namespace IdentityService.Api.Authorization;

// This handler verifies that the user is either in the "Admin" role or is the owner of the resource.
public class OwnerOrAdminHandler : AuthorizationHandler<OwnerOrAdminRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        OwnerOrAdminRequirement requirement)
    {
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var externalIdClaim = context.User.FindFirst("ExternalId")?.Value;
        if (string.IsNullOrEmpty(externalIdClaim)) return Task.CompletedTask;

        if (context.Resource is HttpContext httpContext &&
            httpContext.Request.RouteValues.TryGetValue("externalId", out var routeValue))
            if (routeValue is string routeExternalId &&
                routeExternalId.Equals(externalIdClaim, StringComparison.OrdinalIgnoreCase))
                context.Succeed(requirement);

        return Task.CompletedTask;
    }
}