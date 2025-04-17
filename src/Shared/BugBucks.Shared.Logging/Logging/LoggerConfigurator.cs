using BugBucks.Shared.Logging.Config;
using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;

namespace BugBucks.Shared.Logging;

public static class LoggerConfigurator
{
    public static void ConfigureLogger(IConfiguration configuration)
    {
        var elasticOptions = configuration.GetSection("ElasticsearchOptions").Get<ElasticsearchOptions>();
        var seqOptions = configuration.GetSection("Seq").Get<SeqOptions>();

        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(new LoggingLevelSwitch())
            .Enrich.FromLogContext()
            .WriteTo.Console();

        if (elasticOptions != null && elasticOptions.Enabled && elasticOptions.NodeUris.Any())
        {
            var uris = elasticOptions.NodeUris.Select(u => new Uri(u)).ToArray();
            loggerConfig = loggerConfig.WriteTo.Elasticsearch(
                uris,
                opts =>
                {
                    opts.DataStream = new DataStreamName(
                        elasticOptions.DataStream.Type,
                        elasticOptions.DataStream.Dataset,
                        elasticOptions.DataStream.Namespace);
                    opts.BootstrapMethod = Enum.Parse<BootstrapMethod>(elasticOptions.BootstrapMethod, true);
                    opts.ConfigureChannel = c => c.BufferOptions = new BufferOptions
                        { ExportMaxConcurrency = elasticOptions.BufferOptions.ExportMaxConcurrency };
                });
        }

        if (seqOptions != null && seqOptions.Enabled && !string.IsNullOrEmpty(seqOptions.ServerUrl))
            loggerConfig = loggerConfig.WriteTo.Seq(seqOptions.ServerUrl);

        Log.Logger = loggerConfig.CreateLogger();
    }

    public static void CloseLogger()
    {
        Log.CloseAndFlush();
    }
}