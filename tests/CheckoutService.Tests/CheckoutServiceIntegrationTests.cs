using System.Net.Http.Json;
using CheckoutService.Api;
using CheckoutService.Application.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CheckoutService.Tests;

public class CheckoutServiceIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CheckoutServiceIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ProcessCheckout_ReturnsSuccessResult()
    {
        // Arrange
        var checkoutRequest = new CheckoutRequest
        {
            UserId = Guid.NewGuid(),
            CartItemIds = new List<Guid> { Guid.NewGuid() },
            PaymentMethod = "CreditCard"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/checkout", checkoutRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CheckoutResult>();
        Assert.True(result?.IsSuccessful);
    }
}