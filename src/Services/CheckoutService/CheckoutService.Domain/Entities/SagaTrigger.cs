namespace CheckoutService.Domain.Entities;

/// <summary>
///     Triggers that cause state transitions in the checkout saga.
/// </summary>
public enum SagaTrigger
{
    OrderCreated,
    PaymentSucceeded,
    PaymentFailed,
    InventoryReserveRequested,
    InventoryReserved,
    InventoryFailed,
    OrderCompleted,
    OrderCompensated
}