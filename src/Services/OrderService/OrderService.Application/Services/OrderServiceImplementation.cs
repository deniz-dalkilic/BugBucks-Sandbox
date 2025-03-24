using OrderService.Application.Interfaces;
using OrderService.Domain.Models;

namespace OrderService.Application.Services;

public class OrderServiceImplementation : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderServiceImplementation(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _orderRepository.GetAllOrdersAsync();
    }

    public async Task<Order?> GetOrderByExternalIdAsync(Guid externalId)
    {
        return await _orderRepository.GetOrderByExternalIdAsync(externalId);
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        // Business logic/validations eklenebilir
        return await _orderRepository.CreateOrderAsync(order);
    }

    public async Task UpdateOrderAsync(Order order)
    {
        await _orderRepository.UpdateOrderAsync(order);
    }

    public async Task DeleteOrderAsync(Guid externalId)
    {
        var order = await _orderRepository.GetOrderByExternalIdAsync(externalId);
        if (order != null) await _orderRepository.DeleteOrderAsync(order);
    }
}