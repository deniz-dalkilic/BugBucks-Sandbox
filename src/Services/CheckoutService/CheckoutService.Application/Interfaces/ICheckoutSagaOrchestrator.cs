using BugBucks.Shared.Messaging.Events;

namespace CheckoutService.Application.Services;

/// <summary>
///     Defines handlers for checkout saga events.
/// </summary>
public interface ICheckoutSagaOrchestrator
{
    Task HandleAsync(OrderCreatedEvent evt);
    Task HandleAsync(PaymentSucceededEvent evt);
    Task HandleAsync(PaymentFailedEvent evt);
    Task HandleAsync(InventoryReservedEvent evt);
    Task HandleAsync(InventoryFailedEvent evt);
    Task HandleAsync(OrderCompletedEvent evt);
    Task HandleAsync(OrderCompensatedEvent evt);
}