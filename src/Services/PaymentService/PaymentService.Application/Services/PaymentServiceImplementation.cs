using PaymentService.Application.Interfaces;
using PaymentService.Domain.Enums;
using PaymentService.Domain.Models;

namespace PaymentService.Application.Services;

public class PaymentServiceImplementation : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentServiceImplementation(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Wallet> GetWalletAsync(Guid customerExternalId)
    {
        var wallet = await _paymentRepository.GetWalletByCustomerExternalIdAsync(customerExternalId);
        if (wallet == null)
        {
            wallet = new Wallet { CustomerExternalId = customerExternalId, Balance = 0, BonusBalance = 0 };
            wallet = await _paymentRepository.CreateWalletAsync(wallet);
        }

        return wallet;
    }

    public async Task<Wallet> TopUpWalletAsync(Guid customerExternalId, decimal amount)
    {
        var wallet = await GetWalletAsync(customerExternalId);
        wallet.Balance += amount;
        wallet.UpdatedDate = DateTime.UtcNow;
        await _paymentRepository.UpdateWalletAsync(wallet);
        return wallet;
    }

    public async Task<PaymentTransaction> ProcessPaymentAsync(Guid customerExternalId, decimal amount,
        PaymentMethodType method, string? discountCode = null)
    {
        // Retrieve the customer's wallet
        var wallet = await GetWalletAsync(customerExternalId);

        // For wallet payments, ensure sufficient balance
        if (method == PaymentMethodType.Wallet)
        {
            if (wallet.Balance < amount) throw new InvalidOperationException("Insufficient wallet balance.");
            wallet.Balance -= amount;
            wallet.UpdatedDate = DateTime.UtcNow;
            await _paymentRepository.UpdateWalletAsync(wallet);
        }
        else if (method == PaymentMethodType.ThirdParty)
        {
            // Third-party payment processing integration goes here
        }

        // Create a payment transaction record
        var transaction = new PaymentTransaction
        {
            WalletId = wallet.Id,
            Amount = amount,
            PaymentMethod = method,
            Status = PaymentStatus.Completed
        };
        transaction = await _paymentRepository.CreatePaymentTransactionAsync(transaction);

        // Create an invoice (simplified, tax and discount calculations can be integrated)
        var invoice = new Invoice
        {
            PaymentTransactionId = transaction.Id,
            TotalAmount = amount,
            TaxAmount = 0,
            DiscountAmount = 0
        };
        invoice = await _paymentRepository.CreateInvoiceAsync(invoice);

        transaction.Invoice = invoice;
        return transaction;
    }
}