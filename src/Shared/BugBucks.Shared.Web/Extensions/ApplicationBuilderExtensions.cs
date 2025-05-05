using BugBucks.Shared.Web.Middleware;
using Microsoft.AspNetCore.Builder;

namespace BugBucks.Shared.Web.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseBugBucksWeb(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ErrorHandlingMiddleware>();
        return app;
    }
}