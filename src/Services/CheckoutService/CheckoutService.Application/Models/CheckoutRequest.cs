namespace CheckoutService.Application.Models;

public class CheckoutRequest
{
    public Guid UserId { get; set; }
    public List<Guid> CartItemIds { get; set; } = new();

    public string PaymentMethod { get; set; } = string.Empty;
}