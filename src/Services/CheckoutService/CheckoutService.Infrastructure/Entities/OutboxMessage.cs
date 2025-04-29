namespace CheckoutService.Infrastructure.Entities;

/// <summary>
///     Represents an event message stored in the outbox for reliable delivery.
/// </summary>
public class OutboxMessage
{
    public Guid Id { get; set; } // Primary key
    public string AggregateType { get; set; } // e.g. "CheckoutSaga"
    public Guid AggregateId { get; set; } // OrderId
    public string Type { get; set; } // Event type name
    public string Content { get; set; } // Serialized event JSON
    public DateTime OccurredAt { get; set; }
    public bool Processed { get; set; } // Whether published
    public DateTime? ProcessedAt { get; set; }
}