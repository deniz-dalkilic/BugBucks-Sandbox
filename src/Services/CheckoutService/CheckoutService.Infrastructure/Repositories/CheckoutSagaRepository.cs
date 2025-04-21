using CheckoutService.Domain.Entities;
using CheckoutService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CheckoutService.Infrastructure.Repositories;

public interface ICheckoutSagaRepository
{
    Task<CheckoutSaga> LoadAsync(Guid orderId);
    Task SaveAsync(CheckoutSaga saga);
}

public class CheckoutSagaRepository : ICheckoutSagaRepository
{
    private readonly CheckoutSagaDbContext _db;

    public CheckoutSagaRepository(CheckoutSagaDbContext db)
    {
        _db = db;
    }

    public async Task<CheckoutSaga> LoadAsync(Guid orderId)
    {
        var saga = await _db.CheckoutSagas
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.OrderId == orderId);

        if (saga is null)
        {
            saga = new CheckoutSaga(orderId);
            _db.CheckoutSagas.Add(saga);
            await _db.SaveChangesAsync();
        }

        return saga;
    }

    public async Task SaveAsync(CheckoutSaga saga)
    {
        _db.CheckoutSagas.Update(saga);
        await _db.SaveChangesAsync();
    }
}