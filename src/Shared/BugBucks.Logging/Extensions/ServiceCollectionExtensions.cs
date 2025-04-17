using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace BugBucks.Logging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppLogging(this IServiceCollection services, IConfiguration configuration)
    {
        LoggerConfigurator.ConfigureLogger(configuration);
        services.AddLogging(lb =>
        {
            lb.ClearProviders();
            lb.AddSerilog(dispose: true);
        });
        services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
        return services;
    }
}