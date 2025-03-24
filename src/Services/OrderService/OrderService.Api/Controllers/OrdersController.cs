using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Interfaces;
using OrderService.Domain.Models;

namespace OrderService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // GET: api/orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    // GET: api/orders/{externalId}
    [HttpGet("{externalId:guid}")]
    public async Task<ActionResult<Order>> GetOrder(Guid externalId)
    {
        var order = await _orderService.GetOrderByExternalIdAsync(externalId);
        if (order == null)
            return NotFound();
        return Ok(order);
    }

    // POST: api/orders
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
    {
        if (order.ExternalId == Guid.Empty) order.ExternalId = Guid.NewGuid();
        var createdOrder = await _orderService.CreateOrderAsync(order);
        return CreatedAtAction(nameof(GetOrder), new { externalId = createdOrder.ExternalId }, createdOrder);
    }

    // PUT: api/orders/{externalId}
    [HttpPut("{externalId:guid}")]
    public async Task<IActionResult> UpdateOrder(Guid externalId, [FromBody] Order order)
    {
        if (externalId != order.ExternalId)
            return BadRequest("Order external ID mismatch");

        var existingOrder = await _orderService.GetOrderByExternalIdAsync(externalId);
        if (existingOrder == null)
            return NotFound();

        await _orderService.UpdateOrderAsync(order);
        return Ok(order);
    }

    // DELETE: api/orders/{externalId}
    [HttpDelete("{externalId:guid}")]
    public async Task<IActionResult> DeleteOrder(Guid externalId)
    {
        var order = await _orderService.GetOrderByExternalIdAsync(externalId);
        if (order == null)
            return NotFound();

        await _orderService.DeleteOrderAsync(externalId);
        return NoContent();
    }
}