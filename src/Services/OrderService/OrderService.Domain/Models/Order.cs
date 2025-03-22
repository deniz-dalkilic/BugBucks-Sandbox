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

    // Using CustomerId to associate orders with customers stored elsewhere
    public string CustomerId { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }
    public List<OrderItem> Items { get; set; } = new();

    // Change from single ShippingInfo to a collection of ShippingInfos
    public ICollection<ShippingInfo> ShippingInfos { get; set; } = new List<ShippingInfo>();
}