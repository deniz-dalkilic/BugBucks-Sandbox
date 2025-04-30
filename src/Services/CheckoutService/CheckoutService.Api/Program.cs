using BugBucks.Shared.Logging.Extensions;
using BugBucks.Shared.Messaging.Abstractions.Messaging;
using BugBucks.Shared.Messaging.Contracts.Events;
using BugBucks.Shared.Messaging.Extensions;
using BugBucks.Shared.Messaging.Infrastructure.RabbitMq;
using CheckoutService.Api.HostedServices;
using CheckoutService.Application.Models;
using CheckoutService.Application.Services;
using CheckoutService.Domain.Interfaces;
using CheckoutService.Infrastructure.Data;
using CheckoutService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);

// 1. Logging
builder.AddAppLogging();

// 2. Messaging
builder.Services.AddSharedMessaging(builder.Configuration);

// 3. EF Core
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CheckoutSagaDbContext>(opts =>
    opts.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));

// 4. Repo & Orchestrator
builder.Services.AddScoped<ICheckoutSagaRepository, CheckoutSagaRepository>();
builder.Services.AddScoped<ICheckoutSagaOrchestrator, CheckoutSagaOrchestrator>();

// 5. Hosted Services (Saga Consumer + Outbox)
builder.Services.AddHostedService<CheckoutSagaConsumer>();
builder.Services.AddHostedService<OutboxProcessor>();

var app = builder.Build();

// 6. Startup log
Log.Information("Application starting up...");

// 7. Dev exception page
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

// 8. Correlation ID middleware
app.Use(async (ctx, next) =>
{
    var cid = ctx.Request.Headers["X-Correlation-ID"].ToString();
    if (string.IsNullOrWhiteSpace(cid))
        cid = Guid.NewGuid().ToString();
    using (LogContext.PushProperty("CorrelationId", cid))
    {
        await next();
    }
});

// 9. RabbitMQ Topology

app.Lifetime.ApplicationStarted.Register(async () =>
{
    using var scope = app.Services.CreateScope();
    var factory = scope.ServiceProvider
        .GetRequiredService<IRabbitMqConnectionFactory>();
    var connection = await factory.CreateConnectionAsync();
    await using var channel = await connection.CreateChannelAsync();

    await RabbitMqTopology.DeclareAsync(channel);
});

// 10. API endpoint
app.MapPost("/checkout", async (CheckoutRequest req, IMessagePublisher publisher) =>
{
    var orderId = Guid.NewGuid();
    // Generic event publish
    await publisher.PublishAsync(new OrderCreatedEvent(
        orderId,
        req.CustomerId,
        req.TotalAmount));
    Log.Debug("Checkout requested, OrderId={OrderId}", orderId);
    return Results.Accepted($"/checkout/{orderId}");
});

app.Run();