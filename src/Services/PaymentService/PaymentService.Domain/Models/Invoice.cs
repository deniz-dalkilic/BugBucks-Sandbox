namespace PaymentService.Domain.Models;

public class Invoice
{
    public int Id { get; set; }
    public int PaymentTransactionId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}