using BugBucks.Shared.Logging.Extensions;
using BugBucks.Shared.Messaging.Abstractions.Messaging;
using BugBucks.Shared.Messaging.Contracts.Events;
using BugBucks.Shared.Messaging.Extensions;
using BugBucks.Shared.Messaging.Infrastructure.RabbitMq;
using BugBucks.Shared.VaultClient.Extensions;
using BugBucks.Shared.Web.Extensions;
using CheckoutService.Api.HostedServices;
using CheckoutService.Application.Models;
using CheckoutService.Application.Services;
using CheckoutService.Domain.Interfaces;
using CheckoutService.Infrastructure.Data;
using CheckoutService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppLogging(builder.Configuration, builder.Environment);

builder.Host.UseSerilog();

builder.Services.AddBugBucksWeb();

builder.Services.AddVaultClient();

// Messaging
builder.Services.AddSharedMessaging(builder.Configuration);

// EF Core
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CheckoutSagaDbContext>(opts =>
    opts.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));

// Repo & Orchestrator
builder.Services.AddScoped<ICheckoutSagaRepository, CheckoutSagaRepository>();
builder.Services.AddScoped<ICheckoutSagaOrchestrator, CheckoutSagaOrchestrator>();

// Hosted Services (Saga Consumer + Outbox)
builder.Services.AddHostedService<CheckoutSagaConsumer>();
builder.Services.AddHostedService<OutboxProcessor>();

builder.Services.AddBugBucksWeb();

var app = builder.Build();

// Startup log
Log.Information("Application starting up...");

// Dev exception page
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

// RabbitMQ Topology
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

app.UseSerilogRequestLogging();
app.UseBugBucksWeb();

app.Run();