namespace CheckoutService.Domain.Models;

/// <summary>
///     Domain entity representing a Checkout.
/// </summary>
public class Checkout
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CheckoutDate { get; set; }
    public string Status { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }
}