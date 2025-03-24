using OrderService.Domain.Models;

namespace OrderService.Application.Interfaces;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task<Order?> GetOrderByIdAsync(int id);
    Task<Order?> GetOrderByExternalIdAsync(Guid externalId);
    Task<Order> CreateOrderAsync(Order order);
    Task UpdateOrderAsync(Order order);
    Task DeleteOrderAsync(Order order);
}