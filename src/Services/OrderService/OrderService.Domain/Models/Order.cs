namespace OrderService.Domain.Models;

public class Order
{
    public Order()
    {
    }

    public Order(string customerId, decimal totalAmount)
    {
        CustomerId = customerId;
        TotalAmount = totalAmount;
    }

    public int Id { get; set; }

    // Customer data is managed separately; here we store only the CustomerId.
    public string CustomerId { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }
    public List<OrderItem> Items { get; set; } = new();

    // Shipping and delivery information
    public ShippingInfo? ShippingInfo { get; set; }
}