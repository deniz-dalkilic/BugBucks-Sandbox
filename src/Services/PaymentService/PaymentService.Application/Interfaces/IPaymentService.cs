using PaymentService.Domain.Enums;
using PaymentService.Domain.Models;

namespace PaymentService.Application.Interfaces;

public interface IPaymentService
{
    Task<Wallet> GetWalletAsync(Guid customerExternalId);
    Task<Wallet> TopUpWalletAsync(Guid customerExternalId, decimal amount);

    Task<PaymentTransaction> ProcessPaymentAsync(Guid customerExternalId, decimal amount, PaymentMethodType method,
        string? discountCode = null);
}