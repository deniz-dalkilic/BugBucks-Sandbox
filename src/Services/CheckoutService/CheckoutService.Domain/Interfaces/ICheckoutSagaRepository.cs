using CheckoutService.Domain.Entities;

namespace CheckoutService.Domain.Interfaces;

public interface ICheckoutSagaRepository
{
    Task<CheckoutSaga> LoadAsync(Guid orderId);

    Task SaveAsync(CheckoutSaga saga);
}