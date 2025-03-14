using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace BugBucks.Shared.Logging;

/// <summary>
///     Configures the global logger using configuration settings.
/// </summary>
public static class LoggerConfigurator
{
    public static void ConfigureLogger(IConfiguration configuration)
    {
        // Bind Elasticsearch options from configuration
        var elasticOptions = configuration.GetSection("ElasticsearchOptions").Get<ElasticsearchOptions>();
        var seqServerUrl = configuration["Seq:ServerUrl"];

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Elasticsearch(
                elasticOptions.NodeUris.Select(uri => new Uri(uri)).ToArray(),
                opts =>
                {
                    opts.DataStream = new DataStreamName(
                        elasticOptions.DataStream.Type,
                        elasticOptions.DataStream.Dataset,
                        elasticOptions.DataStream.Namespace);

                    opts.BootstrapMethod =
                        (BootstrapMethod)Enum.Parse(typeof(BootstrapMethod), elasticOptions.BootstrapMethod);


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
                    // Uncomment and configure if authentication is needed.
                    // transport.Authentication(new BasicAuthentication(username, password));
                })
            .WriteTo.Seq(seqServerUrl)
            .CreateLogger();
    }

    public static void CloseLogger()
    {
        Log.CloseAndFlush();
    }
}

// Elasticsearch configuration options
public class ElasticsearchOptions
{
    public string[] NodeUris { get; set; }
    public DataStreamOptions DataStream { get; set; }
    public string BootstrapMethod { get; set; }

    public BufferOptionsConfig BufferOptions { get; set; }

    // New property for template priority
    public int TemplatePriority { get; set; }
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