using BugBucks.Shared.Logging.Interfaces;
using BugBucks.Shared.Messaging.Abstractions.Messaging;
using BugBucks.Shared.Messaging.Contracts.Events;
using CheckoutService.Application.Services;

namespace CheckoutService.Api.HostedServices;

public class CheckoutSagaConsumer : BackgroundService
{
    private readonly IMessageConsumer _consumer;
    private readonly IAppLogger<CheckoutSagaConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public CheckoutSagaConsumer(
        IMessageConsumer consumer,
        IServiceScopeFactory scopeFactory,
        IAppLogger<CheckoutSagaConsumer> logger)
    {
        _consumer = consumer;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _consumer.SubscribeAsync<OrderCreatedEvent>(HandleAsync);
        await _consumer.SubscribeAsync<PaymentSucceededEvent>(HandleAsync);
        await _consumer.SubscribeAsync<PaymentFailedEvent>(HandleAsync);
        await _consumer.SubscribeAsync<InventoryReserveRequestedEvent>(HandleAsync);
        await _consumer.SubscribeAsync<InventoryReservedEvent>(HandleAsync);
        await _consumer.SubscribeAsync<InventoryFailedEvent>(HandleAsync);
        await _consumer.SubscribeAsync<OrderCompletedEvent>(HandleAsync);
        await _consumer.SubscribeAsync<OrderCompensatedEvent>(HandleAsync);

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }

    private async Task HandleAsync<TEvent>(TEvent @event) where TEvent : class
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var orchestrator = scope.ServiceProvider
                .GetRequiredService<ICheckoutSagaOrchestrator>();

            var eventName = typeof(TEvent).Name;
            var orderId = ((dynamic)@event).OrderId as Guid?;
            _logger.LogDebug("Received {Event} for Order={OrderId}", eventName, orderId);

            await ((dynamic)orchestrator).HandleAsync((dynamic)@event);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling saga event {Event}", typeof(TEvent).Name);
        }
    }
}