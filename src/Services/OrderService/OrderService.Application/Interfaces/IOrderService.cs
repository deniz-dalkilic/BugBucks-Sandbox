using OrderService.Domain.Models;

namespace OrderService.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task<Order?> GetOrderByExternalIdAsync(Guid externalId);
    Task<Order> CreateOrderAsync(Order order);
    Task UpdateOrderAsync(Order order);
    Task DeleteOrderAsync(Guid externalId);
}