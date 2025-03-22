namespace OrderService.Domain.Models;

public class ShippingInfo
{
    // Primary key for ShippingInfo
    public int ShippingInfoId { get; set; }

    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ShippingMethod { get; set; } = string.Empty;

    // Optional: Tracking number for third party shipping integration
    public string? TrackingNumber { get; set; }

    // Foreign key to associate with an Order (one-to-many)
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
}