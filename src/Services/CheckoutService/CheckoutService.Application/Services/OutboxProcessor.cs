using System.Text.Json;
using BugBucks.Shared.Logging.Interfaces;
using BugBucks.Shared.Messaging.Abstractions.Messaging;
using BugBucks.Shared.Messaging.Contracts.Events;
using CheckoutService.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CheckoutService.Application.Services;

/// <summary>
///     Background service that publishes pending outbox messages to RabbitMQ
///     using IMessagePublisher.
/// </summary>
public class OutboxProcessor : BackgroundService
{
    private readonly IAppLogger<OutboxProcessor> _logger;
    private readonly IMessagePublisher _publisher;
    private readonly IServiceScopeFactory _scopeFactory;

    public OutboxProcessor(
        IServiceScopeFactory scopeFactory,
        IMessagePublisher publisher,
        IAppLogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _publisher = publisher;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<CheckoutOutboxDbContext>();

                var pending = db.OutboxMessages
                    .Where(x => !x.Processed)
                    .OrderBy(x => x.OccurredAt)
                    .Take(20)
                    .ToList();

                _logger.LogDebug("OutboxProcessor: found {Count} messages", pending.Count);

                foreach (var msg in pending)
                    try
                    {
                        _logger.LogDebug("Publishing outbox message {Id} of type {Type}", msg.Id, msg.Type);

                        object @event = msg.Type switch
                        {
                            nameof(PaymentRequestedEvent) => JsonSerializer.Deserialize<PaymentRequestedEvent>(
                                msg.Content)!,
                            nameof(InventoryReserveRequestedEvent) => JsonSerializer
                                .Deserialize<InventoryReserveRequestedEvent>(msg.Content)!,
                            nameof(InventoryReservedEvent) => JsonSerializer.Deserialize<InventoryReservedEvent>(
                                msg.Content)!,
                            nameof(InventoryFailedEvent) => JsonSerializer.Deserialize<InventoryFailedEvent>(
                                msg.Content)!,
                            nameof(OrderCompletedEvent) =>
                                JsonSerializer.Deserialize<OrderCompletedEvent>(msg.Content)!,
                            nameof(OrderCompensatedEvent) => JsonSerializer.Deserialize<OrderCompensatedEvent>(
                                msg.Content)!,
                            _ => throw new InvalidOperationException($"Unknown event type {msg.Type}")
                        };

                        await _publisher.PublishAsync((dynamic)@event);

                        msg.Processed = true;
                        msg.ProcessedAt = DateTime.UtcNow;
                        db.Update(msg);
                        _logger.LogDebug("Marking outbox message {Id} processed", msg.Id);
                        await db.SaveChangesAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error publishing outbox message {Id}", msg.Id);
                    }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("OutboxProcessor stopping due to cancellation.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unhandled exception in OutboxProcessor");
            throw;
        }
    }
}