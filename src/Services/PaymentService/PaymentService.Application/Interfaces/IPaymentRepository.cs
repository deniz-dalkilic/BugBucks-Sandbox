using PaymentService.Domain.Models;

namespace PaymentService.Application.Interfaces;

public interface IPaymentRepository
{
    Task<Wallet> GetWalletByCustomerExternalIdAsync(Guid customerExternalId,
        CancellationToken cancellationToken = default);

    Task<Wallet> CreateWalletAsync(Wallet wallet, CancellationToken cancellationToken = default);
    Task UpdateWalletAsync(Wallet wallet, CancellationToken cancellationToken = default);

    Task<PaymentTransaction> CreatePaymentTransactionAsync(PaymentTransaction transaction,
        CancellationToken cancellationToken = default);

    Task<PaymentTransaction?> GetPaymentTransactionByExternalIdAsync(Guid externalId,
        CancellationToken cancellationToken = default);

    Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default);
}