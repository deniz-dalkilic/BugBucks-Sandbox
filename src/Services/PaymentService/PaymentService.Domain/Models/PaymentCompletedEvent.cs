namespace PaymentService.Domain.Models;

public class PaymentCompletedEvent
{
    public Guid TransactionExternalId { get; set; }
    public Guid CustomerExternalId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}