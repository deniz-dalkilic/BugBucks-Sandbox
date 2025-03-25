namespace PaymentService.Domain.Models;

public class TaxRate
{
    public int Id { get; set; }
    public string ProductCategory { get; set; } = string.Empty;
    public decimal Rate { get; set; } // e.g. 18 means 18%
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}