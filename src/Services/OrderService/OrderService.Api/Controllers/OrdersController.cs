using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application;
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

    // GET: api/orders/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    // POST: api/orders
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
    {
        var createdOrder = await _orderService.CreateOrderAsync(order);
        return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
    }

    // PUT: api/orders/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
    {
        if (id != order.Id) return BadRequest("Order ID mismatch");

        var existingOrder = await _orderService.GetOrderByIdAsync(id);
        if (existingOrder == null) return NotFound();

        await _orderService.UpdateOrderAsync(order);
        return Ok(order);
    }

    // DELETE: api/orders/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var existingOrder = await _orderService.GetOrderByIdAsync(id);
        if (existingOrder == null) return NotFound();
        await _orderService.DeleteOrderAsync(id);
        return NoContent();
    }
}