using BugBucks.Shared.Logging.Config;
using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;

public static class LoggerConfigurator
{
    public static void ConfigureLogger(IConfiguration config, IHostEnvironment env)
    {
        SelfLog.Enable(msg => Console.WriteLine(msg));

        string MapEnvironmentName(string environmentName)
        {
            return environmentName.ToLowerInvariant() switch
            {
                "development" => "dev",
                "staging" => "stg",
                "production" => "prd",
                _ => environmentName.ToLowerInvariant()
            };
        }

        if (env.IsEnvironment("Test"))
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            return;
        }

        var esOpts = config.GetSection("ElasticsearchOptions").Get<ElasticsearchOptions>()!;
        var seqOpts = config.GetSection("Seq").Get<SeqOptions>()!;
        var otelOpts = config.GetSection("OpenTelemetry").Get<OpenTelemetryOptions>()!;


        var defaultLevel = config.GetValue("Logging:MinimumLevel", LogEventLevel.Debug);
        var elasticLevel = Enum.Parse<LogEventLevel>(esOpts.MinimumLevel, true);
        var seqLevel = Enum.Parse<LogEventLevel>(seqOpts.MinimumLevel, true);
        var otelLevel = Enum.Parse<LogEventLevel>(otelOpts.Logging.MinimumLevel, true);


        var environmentCode = MapEnvironmentName(env.EnvironmentName);

        var logger = new LoggerConfiguration()
            .MinimumLevel.Is(defaultLevel)
            .Enrich.FromLogContext()
            .WriteTo.Console(defaultLevel);

        if (esOpts.Enabled && esOpts.NodeUris.Any())
        {
            var uris = esOpts.NodeUris.Select(u => new Uri(u)).ToArray();
            logger = logger.WriteTo.Elasticsearch(
                uris,
                opts =>
                {
                    opts.DataStream = new DataStreamName(
                        esOpts.DataStream.Type,
                        esOpts.DataStream.Dataset,
                        esOpts.DataStream.Namespace);
                    opts.BootstrapMethod = Enum.Parse<BootstrapMethod>(esOpts.BootstrapMethod, true);
                    opts.ConfigureChannel = c =>
                        c.BufferOptions = new BufferOptions
                            { ExportMaxConcurrency = esOpts.BufferOptions.ExportMaxConcurrency };
                },
                restrictedToMinimumLevel: elasticLevel);
        }

        if (seqOpts.Enabled && !string.IsNullOrWhiteSpace(seqOpts.ServerUrl))
            logger = logger.WriteTo.Seq(
                seqOpts.ServerUrl,
                seqLevel);


        if (otelOpts.Logging.Enabled)
            logger = logger.WriteTo.OpenTelemetry(
                otelOpts.OtlpEndpoint,
                restrictedToMinimumLevel: otelLevel,
                resourceAttributes: new Dictionary<string, object>
                {
                    ["service.name"] = $"{environmentCode}.{env.ApplicationName}",
                    ["deployment.environment"] = env.EnvironmentName
                });

        Log.Logger = logger.CreateLogger();
    }

    public static void CloseLogger()
    {
        Log.CloseAndFlush();
    }
}