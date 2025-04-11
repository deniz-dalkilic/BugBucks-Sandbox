using PaymentService.Domain.Enums;
using PaymentService.Domain.Models;

namespace PaymentService.Application.Interfaces;

public interface IPaymentService
{
    Task<Wallet> GetWalletAsync(Guid customerExternalId, CancellationToken cancellationToken = default);

    Task<Wallet> TopUpWalletAsync(Guid customerExternalId, decimal amount,
        CancellationToken cancellationToken = default);


    Task<PaymentTransaction> ProcessPaymentAsync(Guid customerExternalId, decimal amount, PaymentMethodType method,
        string? discountCode = null, CancellationToken cancellationToken = default);
}