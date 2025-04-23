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

var builder = WebApplication.CreateBuilder(args);

// RabbitMQ DI and config
builder.Services.AddRabbitMq(builder.Configuration);


// EF Core: CheckoutSagaDbContext
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CheckoutSagaDbContext>(opts =>
    opts.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));

// Repositories and orchestrator
builder.Services.AddScoped<ICheckoutSagaRepository, CheckoutSagaRepository>();
builder.Services.AddScoped<ICheckoutSagaOrchestrator, CheckoutSagaOrchestrator>();

// Publisher and consumer
builder.Services.AddSingleton<CheckoutSagaPublisher>();
builder.Services.AddHostedService<CheckoutSagaConsumer>();

var app = builder.Build();

// Declare RabbitMQ topology on startup
app.Lifetime.ApplicationStarted.Register(async () =>
{
    using var scope = app.Services.CreateScope();
    var connection = scope.ServiceProvider.GetRequiredService<IConnection>();
    var channel = await connection.CreateChannelAsync().ConfigureAwait(false);
    await channel.DeclareCheckoutTopologyAsync().ConfigureAwait(false);
});


app.MapPost("/checkout", async (CheckoutRequest req, CheckoutSagaPublisher publisher) =>
{
    var orderId = Guid.NewGuid();

    await publisher.PublishOrderCreatedAsync(orderId, req.CustomerId, req.TotalAmount);
    return Results.Accepted($"/checkout/{orderId}");
});

app.Run();