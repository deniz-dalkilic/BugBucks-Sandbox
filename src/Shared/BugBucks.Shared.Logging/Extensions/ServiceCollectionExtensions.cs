using BugBucks.Shared.Logging.Interfaces;
using BugBucks.Shared.Logging.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BugBucks.Shared.Logging.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppLogging(this IServiceCollection services, IConfiguration cfg,
        IHostEnvironment env)
    {
        services.AddSingleton(typeof(IAppLogger<>), typeof(AppLogger<>));

        LoggerConfigurator.ConfigureLogger(cfg, env);

        return services;
    }
}