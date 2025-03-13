using Serilog;

var builder = WebApplication.CreateBuilder(args);

//0. Clean default log providers
builder.Logging.ClearProviders();

// 1. Add Serilog configuration
builder.Host.UseSerilog((ctx, lc) =>
{
    lc
        .ReadFrom.Configuration(ctx.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(); // We'll expand this later for Elasticsearch
});

// 2. Add Services
builder.Services.AddControllers();

// 3. Build App
var app = builder.Build();

// 4. Configure Middleware
if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();