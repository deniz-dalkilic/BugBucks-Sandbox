using System.Text.Json;
using BugBucks.Shared.Logging.Interfaces;
using BugBucks.Shared.Messaging.Constants;
using BugBucks.Shared.Messaging.Events;
using CheckoutService.Application.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CheckoutService.Api.HostedServices;

public class CheckoutSagaConsumer : BackgroundService
{
    private readonly IChannel _channel;
    private readonly IAppLogger<CheckoutSagaConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public CheckoutSagaConsumer(
        IConnection connection,
        IServiceScopeFactory scopeFactory,
        IAppLogger<CheckoutSagaConsumer> logger)
    {
        // Create channel synchronously at startup
        _channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnEventReceivedAsync;

        _channel.BasicConsumeAsync(
            RabbitMQConstants.CheckoutQueue,
            false,
            consumer, stoppingToken);

        return Task.CompletedTask;
    }

    private async Task OnEventReceivedAsync(object sender, BasicDeliverEventArgs ea)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var orchestrator =
                scope.ServiceProvider.GetRequiredService<ICheckoutSagaOrchestrator>();

            var body = ea.Body.ToArray();
            var eventType = ea.BasicProperties.Type;

            switch (eventType)
            {
                case nameof(OrderCreatedEvent):
                    var created = JsonSerializer.Deserialize<OrderCreatedEvent>(body);
                    if (created != null)
                    {
                        _logger.LogDebug("Consumer received message of type {Type} for OrderId={OrderId}",
                            eventType, created.OrderId);

                        await orchestrator.HandleAsync(created!);
                    }

                    break;
                case nameof(PaymentSucceededEvent):
                    var paid = JsonSerializer.Deserialize<PaymentSucceededEvent>(body);
                    await orchestrator.HandleAsync(paid!);
                    break;
                case nameof(PaymentFailedEvent):
                    var pfailed = JsonSerializer.Deserialize<PaymentFailedEvent>(body);
                    await orchestrator.HandleAsync(pfailed!);
                    break;
                case nameof(InventoryReservedEvent):
                    var invRes = JsonSerializer.Deserialize<InventoryReservedEvent>(body);
                    await orchestrator.HandleAsync(invRes!);
                    break;
                case nameof(InventoryFailedEvent):
                    var invFail = JsonSerializer.Deserialize<InventoryFailedEvent>(body);
                    await orchestrator.HandleAsync(invFail!);
                    break;
                case nameof(OrderCompletedEvent):
                    var completed = JsonSerializer.Deserialize<OrderCompletedEvent>(body);
                    await orchestrator.HandleAsync(completed!);
                    break;
                case nameof(OrderCompensatedEvent):
                    var compensated = JsonSerializer.Deserialize<OrderCompensatedEvent>(body);
                    await orchestrator.HandleAsync(compensated!);
                    break;
            }

            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        }
        catch
        {
            await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
        }
    }
}