using Microsoft.AspNetCore.Builder;

namespace BugBucks.Shared.Observability.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseBugBucksObservability(this IApplicationBuilder app)
    {
        app.UseOpenTelemetryPrometheusScrapingEndpoint();
        return app;
    }
}