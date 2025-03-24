using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces;
using OrderService.Domain.Models;
using OrderService.Infrastructure.Data;

namespace OrderService.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingInfos)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingInfos)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task UpdateOrderAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task<Order?> GetOrderByExternalIdAsync(Guid externalId)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingInfos)
            .FirstOrDefaultAsync(o => o.ExternalId == externalId);
    }

    public async Task DeleteOrderAsync(Order order)
    {
        // Soft-delete: mark as deleted
        order.IsDeleted = true;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
}