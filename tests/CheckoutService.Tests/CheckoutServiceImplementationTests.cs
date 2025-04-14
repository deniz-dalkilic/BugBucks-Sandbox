using CheckoutService.Application.Interfaces;
using CheckoutService.Application.Models;
using CheckoutService.Application.Services;
using Moq;

namespace CheckoutService.Tests;

public class CheckoutServiceImplementationTests
{
    [Fact]
    public async Task ProcessCheckoutAsync_ShouldReturnSuccessfulResult()
    {
        // Arrange
        var mockRepo = new Mock<ICheckoutRepository>();
        // Setup repository mock (if needed for verification)
        var service = new CheckoutServiceImplementation(mockRepo.Object);
        var checkoutRequest = new CheckoutRequest
        {
            UserId = Guid.NewGuid(),
            CartItemIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
            PaymentMethod = "CreditCard"
        };

        // Act
        var result = await service.ProcessCheckoutAsync(checkoutRequest);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.False(string.IsNullOrEmpty(result.Message));
    }
}