using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace BugBucks.Shared.Logging;

public static class LoggerConfigurator
{
    public static void ConfigureLogger(IConfiguration configuration)
    {
        // Read Elasticsearch options if available
        var elasticOptions = configuration.GetSection("ElasticsearchOptions").Get<ElasticsearchOptions>();

        // Read Seq configuration
        var seqServerUrl = configuration.GetValue<string>("Seq:ServerUrl");
        var seqEnabled = configuration.GetValue("Seq:Enabled", true);

        // Build base logger configuration
        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

        // Add Elasticsearch sink if enabled and NodeUris are provided
        if (elasticOptions != null && elasticOptions.Enabled &&
            elasticOptions.NodeUris != null && elasticOptions.NodeUris.Any())
        {
            var nodeUris = elasticOptions.NodeUris.Select(uri => new Uri(uri)).ToArray();
            loggerConfig = loggerConfig.WriteTo.Elasticsearch(
                nodeUris,
                opts =>
                {
                    opts.DataStream = new DataStreamName(
                        elasticOptions.DataStream.Type,
                        elasticOptions.DataStream.Dataset,
                        elasticOptions.DataStream.Namespace);
                    opts.BootstrapMethod = Enum.Parse<BootstrapMethod>(elasticOptions.BootstrapMethod, true);
                    opts.ConfigureChannel = channelOpts =>
                    {
                        channelOpts.BufferOptions = new BufferOptions
                        {
                            ExportMaxConcurrency = elasticOptions.BufferOptions.ExportMaxConcurrency
                        };
                    };
                },
                transport =>
                {
                    // Configure transport authentication if needed.
                    // Example: transport.Authentication(new BasicAuthentication(username, password));
                });
        }

        // Add Seq sink if enabled
        if (seqEnabled && !string.IsNullOrEmpty(seqServerUrl)) loggerConfig = loggerConfig.WriteTo.Seq(seqServerUrl);

        Log.Logger = loggerConfig.CreateLogger();
    }

    public static void CloseLogger()
    {
        Log.CloseAndFlush();
    }
}

// Strongly typed options for Elasticsearch configuration
public class ElasticsearchOptions
{
    public bool Enabled { get; set; } = true;
    public string[] NodeUris { get; set; }
    public DataStreamOptions DataStream { get; set; }
    public string BootstrapMethod { get; set; }
    public BufferOptionsConfig BufferOptions { get; set; }
}

public class DataStreamOptions
{
    public string Type { get; set; }
    public string Dataset { get; set; }
    public string Namespace { get; set; }
}

public class BufferOptionsConfig
{
    public int ExportMaxConcurrency { get; set; }
}