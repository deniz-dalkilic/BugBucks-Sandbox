using OrderService.Application.Interfaces;
using OrderService.Domain.Models;

namespace OrderService.Application;

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

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _orderRepository.GetOrderByIdAsync(id);
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        return await _orderRepository.CreateOrderAsync(order);
    }

    public async Task UpdateOrderAsync(Order order)
    {
        await _orderRepository.UpdateOrderAsync(order);
    }

    public async Task DeleteOrderAsync(int id)
    {
        await _orderRepository.DeleteOrderAsync(id);
    }
}