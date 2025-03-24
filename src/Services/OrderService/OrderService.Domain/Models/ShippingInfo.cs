namespace OrderService.Domain.Models;

public class ShippingInfo
{
    public int ShippingInfoId { get; set; }

    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ShippingMethod { get; set; } = string.Empty;
    public string? TrackingNumber { get; set; }

    // Foreign key to Order
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    // Soft-delete flag
    public bool IsDeleted { get; set; } = false;
}