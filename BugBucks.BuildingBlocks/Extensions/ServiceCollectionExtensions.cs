using BugBucks.BuildingBlocks.Constants;
using BugBucks.BuildingBlocks.Providers;

namespace BugBucks.BuildingBlocks.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddBuildingBlocks(this WebApplicationBuilder builder)
    {
        // Add shared providers
        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        builder.Services.AddSingleton<IGuidProvider, GuidProvider>();
        // Add controllers support
        builder.Services.AddControllers();
        return builder;
    }

    public static WebApplication UseBuildingBlocks(this WebApplication app)
    {
        // Use common middleware and endpoints
        app.UseRouting();
#pragma warning disable ASP0014
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks(ApiRoutes.Health);
            endpoints.MapHealthChecks(ApiRoutes.Readiness);
        });
#pragma warning restore ASP0014
        return app;
    }
}