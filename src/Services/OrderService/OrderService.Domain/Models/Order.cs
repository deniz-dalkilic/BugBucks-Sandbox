namespace OrderService.Domain.Models;

public class Order
{
    // Internal primary key
    public int Id { get; set; }

    // External identifier used in API calls
    public Guid ExternalId { get; set; } = Guid.NewGuid();

    // Soft-delete flag
    public bool IsDeleted { get; set; } = false;

    // Order details
    public string CustomerId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public List<OrderItem> Items { get; set; } = new();

    // One-to-many relationship: an order can have multiple shipping records
    public ICollection<ShippingInfo> ShippingInfos { get; set; } = new List<ShippingInfo>();
}