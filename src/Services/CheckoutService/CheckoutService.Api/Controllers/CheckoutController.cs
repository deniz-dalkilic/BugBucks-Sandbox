using CheckoutService.Application.Interfaces;
using CheckoutService.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    /// <summary>
    ///     Initiates the checkout process.
    /// </summary>
    /// <param name="request">Checkout request details</param>
    /// <returns>A checkout result.</returns>
    [HttpPost]
    public async Task<IActionResult> ProcessCheckout([FromBody] CheckoutRequest request)
    {
        // Process the checkout by delegating to the application service
        var result = await _checkoutService.ProcessCheckoutAsync(request);
        return Ok(result);
    }
}