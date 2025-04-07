using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Enums;

namespace PaymentService.Api.Controllers;

[Route("api")]
[ApiController]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // GET: api/payment/wallet/{customerExternalId}
    [HttpGet("wallet/{customerExternalId:guid}")]
    public async Task<IActionResult> GetWallet(Guid customerExternalId)
    {
        var wallet = await _paymentService.GetWalletAsync(customerExternalId);
        if (wallet == null)
            return NotFound();
        return Ok(wallet);
    }

    // POST: api/payment/topup
    [HttpPost("topup")]
    public async Task<IActionResult> TopUpWallet([FromBody] TopUpRequest request)
    {
        var wallet = await _paymentService.TopUpWalletAsync(request.CustomerExternalId, request.Amount);
        return Ok(wallet);
    }

    // POST: api/payment/process
    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest request)
    {
        var transaction = await _paymentService.ProcessPaymentAsync(request.CustomerExternalId, request.Amount,
            request.PaymentMethod, request.DiscountCode);
        return Ok(transaction);
    }
}

public class TopUpRequest
{
    public Guid CustomerExternalId { get; set; }
    public decimal Amount { get; set; }
}

public class ProcessPaymentRequest
{
    public Guid CustomerExternalId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethodType PaymentMethod { get; set; }
    public string? DiscountCode { get; set; }
}