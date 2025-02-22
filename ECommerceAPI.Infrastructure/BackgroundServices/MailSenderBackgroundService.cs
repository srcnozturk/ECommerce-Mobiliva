using ECommerceAPI.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text.Json;
using ECommerceAPI.Core.Dtos;
using System.Text;

namespace ECommerceAPI.Infrastructure.BackgroundServices;

public class MailSenderBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<MailSenderBackgroundService> _logger;

    public MailSenderBackgroundService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<MailSenderBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        try
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:Host"],
                UserName = configuration["RabbitMQ:Username"],
                Password = configuration["RabbitMQ:Password"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("SendMail", durable: true, exclusive: false, autoDelete: false);
            _logger.LogInformation("RabbitMQ connection established for MailSenderBackgroundService");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ connection for MailSenderBackgroundService");
            throw;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MailSenderBackgroundService started");
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var emailMessage = JsonSerializer.Deserialize<EmailMessageDto>(message);

            _logger.LogInformation("Processing email message for recipient: {Recipient}", emailMessage.To);

            using (var scope = _serviceProvider.CreateScope())
            {
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                try
                {
                    await emailService.SendEmailAsync(emailMessage);
                    _channel.BasicAck(ea.DeliveryTag, false);
                    _logger.LogInformation("Email sent successfully to: {Recipient}", emailMessage.To);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send email to: {Recipient}", emailMessage.To);
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            }
        };

        _channel.BasicConsume(queue: "SendMail",
            autoAck: false,
            consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation("MailSenderBackgroundService stopping");
    }
}
