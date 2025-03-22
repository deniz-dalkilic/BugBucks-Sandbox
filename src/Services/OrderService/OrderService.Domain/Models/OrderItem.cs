namespace OrderService.Domain.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; } // Association with Order
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public decimal TotalPrice => Quantity * UnitPrice;
}