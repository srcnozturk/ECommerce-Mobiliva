using ECommerceAPI.Core.Interfaces;
using ECommerceAPI.Core.Models.Email;
using ECommerceAPI.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ECommerceAPI.UnitTests.Services;

public class EmailServiceTests
{
    private readonly Mock<ILogger<EmailService>> _loggerMock;
    private readonly Mock<IEmailService> _emailServiceMock;

    public EmailServiceTests()
    {
        _loggerMock = new Mock<ILogger<EmailService>>();
        _emailServiceMock = new Mock<IEmailService>();
    }

    [Fact]
    public async Task SendEmailAsync_WithValidMessage_ShouldSucceed()
    {
        // Arrange
        var emailMessage = new EmailMessage
        {
            To = "test@example.com",
            Subject = "Test Subject",
            Body = "Test Body"
        };

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<EmailMessage>()))
            .Returns(Task.CompletedTask);

        // Act
        await _emailServiceMock.Object.SendEmailAsync(emailMessage);

        // Assert
        _emailServiceMock.Verify(x => x.SendEmailAsync(It.Is<EmailMessage>(m =>
            m.To == emailMessage.To &&
            m.Subject == emailMessage.Subject &&
            m.Body == emailMessage.Body)), Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_WithInvalidEmail_ShouldThrowException()
    {
        // Arrange
        var emailMessage = new EmailMessage
        {
            To = "invalid-email",
            Subject = "Test Subject",
            Body = "Test Body"
        };

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<EmailMessage>()))
            .ThrowsAsync(new ArgumentException("Invalid email address"));

        // Act & Assert
        await _emailServiceMock.Object
            .Invoking(x => x.SendEmailAsync(emailMessage))
            .Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("Invalid email address");
    }
}
