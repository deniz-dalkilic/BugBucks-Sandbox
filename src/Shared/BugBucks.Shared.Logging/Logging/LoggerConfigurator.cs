using BugBucks.Shared.Logging.Config;
using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

public static class LoggerConfigurator
{
    public static void ConfigureLogger(IConfiguration config, IHostEnvironment env)
    {
        if (env.IsEnvironment("Test"))
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            return;
        }


        var defaultLevel = config.GetValue("Logging:MinimumLevel", LogEventLevel.Debug);
        var elasticLevel = config.GetValue("ElasticsearchOptions:MinimumLevel", defaultLevel);
        var seqLevel = config.GetValue("Seq:MinimumLevel", defaultLevel);

        var logger = new LoggerConfiguration()
            .MinimumLevel.Is(defaultLevel)
            .Enrich.FromLogContext()
            .WriteTo.Console(defaultLevel);

        var esOpts = config.GetSection("ElasticsearchOptions").Get<ElasticsearchOptions>()!;
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

        var seqOpts = config.GetSection("Seq").Get<SeqOptions>()!;
        if (seqOpts.Enabled && !string.IsNullOrWhiteSpace(seqOpts.ServerUrl))
            logger = logger.WriteTo.Seq(
                seqOpts.ServerUrl,
                seqLevel);

        Log.Logger = logger.CreateLogger();
    }

    public static void CloseLogger()
    {
        Log.CloseAndFlush();
    }
}