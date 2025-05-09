using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace BugBucks.Shared.Observability.Extensions;

public static class ServiceCollectionExtensions
{
    private const string ServiceNameKey = "ServiceName";

    public static IServiceCollection AddBugBucksObservability(
        this IServiceCollection services,
        IConfiguration cfg,
        string serviceName)
    {
        Log.Logger = Log.Logger.ForContext(ServiceNameKey, serviceName);

        services.AddOpenTelemetry()
            .ConfigureResource(r => r
                .AddService(serviceName,
                    serviceVersion: "1.0.0"))
            .WithTracing(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(cfg["OpenTelemetry:OtlpEndpoint"] ?? "http://collector:4317");
                    });
            })
            .WithMetrics(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation()
                    .AddPrometheusExporter();
            });

        return services;
    }
}