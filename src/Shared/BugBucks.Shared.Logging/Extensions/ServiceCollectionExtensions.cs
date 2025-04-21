using BugBucks.Shared.Logging.Interfaces;
using BugBucks.Shared.Logging.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BugBucks.Shared.Logging.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddAppLogging(this WebApplicationBuilder builder)
    {
        // Configure Serilog from configuration
        LoggerConfigurator.ConfigureLogger(builder.Configuration);

        // Replace default logging
        builder.Host.UseSerilog();

        // Register IAppLogger<T>
        builder.Services.AddSingleton(typeof(IAppLogger<>), typeof(AppLogger<>));

        return builder;
    }
}