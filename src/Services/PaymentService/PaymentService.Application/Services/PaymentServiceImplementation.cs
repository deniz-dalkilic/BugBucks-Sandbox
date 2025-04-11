using System.Text.Json;
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

    public async Task<Wallet> GetWalletAsync(Guid customerExternalId, CancellationToken cancellationToken = default)
    {
        var wallet = await _paymentRepository.GetWalletByCustomerExternalIdAsync(customerExternalId, cancellationToken);
        if (wallet == null)
        {
            wallet = new Wallet
            {
                CustomerExternalId = customerExternalId,
                Balance = 0,
                BonusBalance = 0,
                CreatedDate = DateTime.UtcNow
            };
            wallet = await _paymentRepository.CreateWalletAsync(wallet, cancellationToken);
        }

        return wallet;
    }

    public async Task<Wallet> TopUpWalletAsync(Guid customerExternalId, decimal amount,
        CancellationToken cancellationToken = default)
    {
        var wallet = await GetWalletAsync(customerExternalId, cancellationToken);
        wallet.Balance += amount;
        wallet.UpdatedDate = DateTime.UtcNow;
        await _paymentRepository.UpdateWalletAsync(wallet, cancellationToken);
        return wallet;
    }

    public async Task<PaymentTransaction> ProcessPaymentAsync(
        Guid customerExternalId,
        decimal amount,
        PaymentMethodType method,
        string? discountCode = null,
        CancellationToken cancellationToken = default)
    {
        var wallet = await GetWalletAsync(customerExternalId, cancellationToken);

        if (method == PaymentMethodType.Wallet)
        {
            if (wallet.Balance < amount)
                throw new InvalidOperationException("Insufficient wallet balance.");
            wallet.Balance -= amount;
            wallet.UpdatedDate = DateTime.UtcNow;
            await _paymentRepository.UpdateWalletAsync(wallet, cancellationToken);
        }
        else if (method == PaymentMethodType.ThirdParty)
        {
            // Integration with a third-party provider
        }

        var transaction = new PaymentTransaction
        {
            WalletId = wallet.Id,
            Amount = amount,
            PaymentMethod = method,
            Status = PaymentStatus.Completed,
            CreatedDate = DateTime.UtcNow,
            ExternalId = Guid.NewGuid()
        };

        transaction = await _paymentRepository.CreatePaymentTransactionAsync(transaction, cancellationToken);

        var invoice = new Invoice
        {
            PaymentTransactionId = transaction.Id,
            TotalAmount = amount,
            TaxAmount = 0,
            DiscountAmount = 0,
            CreatedDate = DateTime.UtcNow
        };

        invoice = await _paymentRepository.CreateInvoiceAsync(invoice, cancellationToken);
        transaction.Invoice = invoice;

        var paymentEvent = new PaymentCompletedEvent
        {
            TransactionExternalId = transaction.ExternalId,
            CustomerExternalId = customerExternalId,
            Amount = amount,
            Timestamp = DateTime.UtcNow
        };

        var eventJson = JsonSerializer.Serialize(paymentEvent);


        return transaction;
    }
}