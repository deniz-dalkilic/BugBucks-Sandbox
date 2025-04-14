using CheckoutService.Application.Models;

namespace CheckoutService.Application.Interfaces;

public interface ICheckoutService
{
    Task<CheckoutResult> ProcessCheckoutAsync(CheckoutRequest request);
}