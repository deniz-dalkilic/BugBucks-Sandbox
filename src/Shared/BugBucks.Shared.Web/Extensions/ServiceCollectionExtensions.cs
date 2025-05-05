using BugBucks.Shared.Web.Middleware;
using Microsoft.Extensions.DependencyInjection;

namespace BugBucks.Shared.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBugBucksWeb(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<CorrelationIdMiddleware>();
        services.AddTransient<RequestLoggingMiddleware>();
        services.AddTransient<ErrorHandlingMiddleware>();
        return services;
    }
}