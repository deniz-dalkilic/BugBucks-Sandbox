namespace CheckoutService.Domain.Entities;

/// <summary>
///     Represents the possible states of a checkout saga.
/// </summary>
public enum SagaState
{
    Pending,
    PaymentProcessing,
    PaymentSucceeded,
    InventoryReserve,
    InventoryReserved,
    Completed,
    Compensated
}