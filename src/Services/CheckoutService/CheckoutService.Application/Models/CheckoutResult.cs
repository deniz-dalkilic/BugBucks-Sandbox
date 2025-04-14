namespace CheckoutService.Application.Models;

public class CheckoutResult
{
    public bool IsSuccessful { get; set; }
    public string? OrderId { get; set; }
    public string? TransactionId { get; set; }
    public string Message { get; set; } = string.Empty;
}