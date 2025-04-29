using BugBucks.Shared.Logging.Extensions;
using BugBucks.Shared.Messaging.Extensions;
using BugBucks.Shared.Messaging.Services;
using BugBucks.Shared.Messaging.Topology;
using CheckoutService.Api.HostedServices;
using CheckoutService.Application.Models;
using CheckoutService.Application.Services;
using CheckoutService.Infrastructure.Data;
using CheckoutService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Serilog;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.AddAppLogging();

// RabbitMQ DI and config
builder.Services.AddRabbitMq(builder.Configuration);

// EF Core: CheckoutSagaDbContext
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CheckoutSagaDbContext>(opts =>
    opts.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));

// Repositories and orchestrator
builder.Services.AddScoped<ICheckoutSagaRepository, CheckoutSagaRepository>();
builder.Services.AddScoped<ICheckoutSagaOrchestrator, CheckoutSagaOrchestrator>();

// Saga Publisher and Consumer
builder.Services.AddSingleton<CheckoutSagaPublisher>();
builder.Services.AddHostedService<CheckoutSagaConsumer>();

// Outbox Processor for reliable message delivery
builder.Services.AddHostedService<OutboxProcessor>();

var app = builder.Build();

// Log startup
Log.Information("Application starting up...");

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

// Use correlation ID middleware for logging
app.Use(async (context, next) =>
{
    var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                        ?? Guid.NewGuid().ToString();
    using (LogContext.PushProperty("CorrelationId", correlationId))
    {
        await next();
    }
});

// Declare RabbitMQ topology on startup
app.Lifetime.ApplicationStarted.Register(async () =>
{
    using var scope = app.Services.CreateScope();
    var connection = scope.ServiceProvider.GetRequiredService<IConnection>();
    var channel = await connection.CreateChannelAsync().ConfigureAwait(false);
    await channel.DeclareCheckoutTopologyAsync().ConfigureAwait(false);
});

// API endpoint for starting checkout
app.MapPost("/checkout", async (CheckoutRequest req, CheckoutSagaPublisher publisher) =>
{
    var orderId = Guid.NewGuid();
    // Publish initial OrderCreatedEvent
    await publisher.PublishOrderCreatedAsync(orderId, req.CustomerId, req.TotalAmount);
    Log.Debug("Received checkout request for OrderId={OrderId}", orderId);
    return Results.Accepted($"/checkout/{orderId}");
});

app.Run();