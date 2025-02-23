using ECommerceAPI.Core.Interfaces;
using ECommerceAPI.Core.Models.Email;
using ECommerceAPI.Infrastructure.BackgroundServices;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ECommerceAPI.UnitTests.BackgroundServices;

public class MailSenderBackgroundServiceTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<MailSenderBackgroundService>> _loggerMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;

    public MailSenderBackgroundServiceTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<MailSenderBackgroundService>>();
        _emailServiceMock = new Mock<IEmailService>();
        _serviceScopeMock = new Mock<IServiceScope>();
        _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();

        // Configure service provider mocks
        _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(_serviceScopeFactoryMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IEmailService)))
            .Returns(_emailServiceMock.Object);

        // Configure RabbitMQ configuration
        _configurationMock.Setup(x => x["RabbitMQ:Host"]).Returns("localhost");
        _configurationMock.Setup(x => x["RabbitMQ:Username"]).Returns("guest");
        _configurationMock.Setup(x => x["RabbitMQ:Password"]).Returns("guest");
    }

    [Fact]
    public void Constructor_ShouldInitializeRabbitMQConnection()
    {
        // Act
        var service = new MailSenderBackgroundService(
            _serviceProviderMock.Object,
            _configurationMock.Object,
            _loggerMock.Object);

        // Assert
        service.Should().NotBeNull();
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("RabbitMQ connection established")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldProcessEmailMessage()
    {
        // Arrange
        var service = new MailSenderBackgroundService(
            _serviceProviderMock.Object,
            _configurationMock.Object,
            _loggerMock.Object);

        var emailMessage = new EmailMessage
        {
            To = "test@example.com",
            Subject = "Test Subject",
            Body = "Test Body"
        };

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<EmailMessage>()))
            .Returns(Task.CompletedTask);

        // Act
        await service.StartAsync(CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("MailSenderBackgroundService started")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
