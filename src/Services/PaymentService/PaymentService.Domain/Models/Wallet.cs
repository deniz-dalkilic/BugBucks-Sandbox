namespace PaymentService.Domain.Models;

public class Wallet
{
    public int Id { get; set; }
    public Guid ExternalId { get; set; } = Guid.NewGuid();
    public Guid CustomerExternalId { get; set; }
    public decimal Balance { get; set; }
    public decimal BonusBalance { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }
}