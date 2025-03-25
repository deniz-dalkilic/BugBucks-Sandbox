using PaymentService.Domain.Models;

namespace PaymentService.Application.Interfaces;

public interface IPaymentRepository
{
    Task<Wallet> GetWalletByCustomerExternalIdAsync(Guid customerExternalId);
    Task<Wallet> CreateWalletAsync(Wallet wallet);
    Task UpdateWalletAsync(Wallet wallet);

    Task<PaymentTransaction> CreatePaymentTransactionAsync(PaymentTransaction transaction);
    Task<PaymentTransaction?> GetPaymentTransactionByExternalIdAsync(Guid externalId);

    Task<Invoice> CreateInvoiceAsync(Invoice invoice);
}