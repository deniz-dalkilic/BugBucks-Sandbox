namespace BugBucks.Shared.Messaging.Events;

// Events for Checkout Saga
public record OrderCreatedEvent(Guid OrderId, Guid CustomerId, decimal TotalAmount);

public record PaymentRequestedEvent(Guid OrderId, string PaymentDetails, string IdempotencyKey);

public record PaymentSucceededEvent(Guid OrderId, string TransactionId);

public record PaymentFailedEvent(Guid OrderId, string ErrorCode);

public record InventoryReserveRequestedEvent(Guid OrderId, IReadOnlyCollection<InventoryItem> Items);

public record InventoryReservedEvent(Guid OrderId);

public record InventoryFailedEvent(Guid OrderId, string Reason);

public record OrderCompletedEvent(Guid OrderId);

public record OrderCompensatedEvent(Guid OrderId, string Reason);

// Supporting DTO
public record InventoryItem(Guid ProductId, int Quantity);