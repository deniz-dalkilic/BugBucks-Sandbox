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

    public async Task<Wallet> GetWalletByCustomerExternalIdAsync(Guid customerExternalId)
    {
        return await _context.Wallets.FirstOrDefaultAsync(w => w.CustomerExternalId == customerExternalId);
    }

    public async Task<Wallet> CreateWalletAsync(Wallet wallet)
    {
        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }

    public async Task UpdateWalletAsync(Wallet wallet)
    {
        _context.Wallets.Update(wallet);
        await _context.SaveChangesAsync();
    }

    public async Task<PaymentTransaction> CreatePaymentTransactionAsync(PaymentTransaction transaction)
    {
        _context.PaymentTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<PaymentTransaction?> GetPaymentTransactionByExternalIdAsync(Guid externalId)
    {
        return await _context.PaymentTransactions.FirstOrDefaultAsync(t => t.ExternalId == externalId);
    }

    public async Task<Invoice> CreateInvoiceAsync(Invoice invoice)
    {
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();
        return invoice;
    }
}