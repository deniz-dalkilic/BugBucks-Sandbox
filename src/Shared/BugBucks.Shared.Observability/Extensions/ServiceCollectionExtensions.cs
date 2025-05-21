using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Logs;
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

        var otlpEndpoint = new Uri(cfg["OpenTelemetry:OtlpEndpoint"]
                                   ?? "http://otel-collector:4317");

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
                    .AddOtlpExporter(opt => opt.Endpoint = otlpEndpoint);
            })
            .WithMetrics(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation()
                    .AddPrometheusExporter()
                    .AddOtlpExporter(opt => opt.Endpoint = otlpEndpoint);
            })
            .WithLogging(builder =>
            {
                builder
                    .AddOtlpExporter(opt => opt.Endpoint = otlpEndpoint);
            });

        return services;
    }
}