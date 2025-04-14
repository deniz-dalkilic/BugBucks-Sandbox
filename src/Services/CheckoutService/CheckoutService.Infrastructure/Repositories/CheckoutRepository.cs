using CheckoutService.Application.Interfaces;
using CheckoutService.Domain.Models;
using CheckoutService.Infrastructure.Data;

namespace CheckoutService.Infrastructure.Repositories;

public class CheckoutRepository : ICheckoutRepository
{
    private readonly CheckoutDbContext _dbContext;

    public CheckoutRepository(CheckoutDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddCheckoutAsync(Checkout checkout)
    {
        await _dbContext.Checkouts.AddAsync(checkout);
        await _dbContext.SaveChangesAsync();
    }
}