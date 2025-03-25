using System.Net.Http.Json;
using FluentAssertions;
using IdentityService.Api;
using IdentityService.Api.Models;
using IdentityService.Tests.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IdentityService.Tests;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<EntryPoint>>
{
    private readonly HttpClient _client;

    public AuthControllerTests(WebApplicationFactory<EntryPoint> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenUserIsRegistered()
    {
        // Arrange
        var registerModel = new RegisterRequest
        {
            UserName = "testuser3",
            Email = "testuser3@example.com",
            Password = "Test@123",
            FullName = "Test User3",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerModel);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("registered successfully");
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        // Create a test user first.
        var registerModel = new RegisterRequest
        {
            UserName = "loginuser2",
            Email = "loginuser2@example.com",
            Password = "Test@123",
            FullName = "Login User2",
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerModel);

        var loginModel = new LoginModel
        {
            UserName = "loginuser2",
            Password = "Test@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginModel);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        // Response'u strongly-typed model ile deserialize ediyoruz.
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        loginResponse.Should().NotBeNull();
        loginResponse.Token.Should().NotBeNullOrEmpty();
    }
}