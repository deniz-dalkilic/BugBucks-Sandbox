using CheckoutService.Domain.Models;

namespace CheckoutService.Application.Interfaces;

public interface ICheckoutRepository
{
    Task AddCheckoutAsync(Checkout checkout);
    // Additional methods like GetCheckout, UpdateCheckout can be added here.
}