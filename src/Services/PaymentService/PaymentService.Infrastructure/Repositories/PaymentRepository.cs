using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Models;
using PaymentService.Infrastructure.Data;

namespace PaymentService.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _context;

    public PaymentRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<Wallet> GetWalletByCustomerExternalIdAsync(Guid customerExternalId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Wallets.FirstOrDefaultAsync(w => w.CustomerExternalId == customerExternalId,
            cancellationToken);
    }

    public async Task<Wallet> CreateWalletAsync(Wallet wallet, CancellationToken cancellationToken = default)
    {
        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync(cancellationToken);
        return wallet;
    }

    public async Task UpdateWalletAsync(Wallet wallet, CancellationToken cancellationToken = default)
    {
        _context.Wallets.Update(wallet);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<PaymentTransaction> CreatePaymentTransactionAsync(PaymentTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        _context.PaymentTransactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);
        return transaction;
    }

    public async Task<PaymentTransaction?> GetPaymentTransactionByExternalIdAsync(Guid externalId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PaymentTransactions.FirstOrDefaultAsync(t => t.ExternalId == externalId,
            cancellationToken);
    }

    public async Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync(cancellationToken);
        return invoice;
    }
}