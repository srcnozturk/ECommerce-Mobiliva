using System.Net;
using System.Net.Http.Json;
using ECommerceAPI.Core.Models.Email;
using Microsoft.AspNetCore.Mvc.Testing;
using ECommerceAPI.API;

namespace ECommerceAPI.IntegrationTests.Controllers;

public class EmailControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public EmailControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task SendEmail_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var emailMessage = new EmailMessage
        {
            To = "test@example.com",
            Subject = "Integration Test",
            Body = "This is an integration test email"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/email/send", emailMessage);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SendEmail_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var emailMessage = new EmailMessage
        {
            To = "invalid-email",
            Subject = "Integration Test",
            Body = "This is an integration test email"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/email/send", emailMessage);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
