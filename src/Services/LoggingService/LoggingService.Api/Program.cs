using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using LoggingService.Api.Configurations;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Bind ElasticsearchOptions from configuration
var elasticOptions = builder.Configuration
    .GetSection("ElasticsearchOptions")
    .Get<ElasticsearchOptions>();

var seqServerUrl = builder.Configuration["Seq:ServerUrl"];

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
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

            opts.BootstrapMethod = (BootstrapMethod)Enum.Parse(typeof(BootstrapMethod), elasticOptions.BootstrapMethod);

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
            // transport.Authentication(new BasicAuthentication(username, password)); // Basic Auth
            // transport.Authentication(new ApiKey(base64EncodedApiKey)); // ApiKey
        })
    .WriteTo.Seq(seqServerUrl)
    .CreateLogger();

// Add services to the container.
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();