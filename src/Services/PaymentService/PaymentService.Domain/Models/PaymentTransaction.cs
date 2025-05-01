using PaymentService.Domain.Enums;

namespace PaymentService.Domain.Models;

public class PaymentTransaction
{
    public int Id { get; set; }
    public Guid ExternalId { get; set; } = Guid.NewGuid();
    public int WalletId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethodType PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public Invoice Invoice { get; set; }
}