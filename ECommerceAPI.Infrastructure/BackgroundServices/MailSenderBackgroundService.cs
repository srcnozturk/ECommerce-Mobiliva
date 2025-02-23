using Microsoft.Extensions.DependencyInjection;

namespace ECommerceAPI.Infrastructure.BackgroundServices;

/// <summary>
/// Background service that listens for email messages from RabbitMQ queue and processes them.
/// Implements reliable email sending with acknowledgment and retry mechanisms.
/// </summary>
public class MailSenderBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<MailSenderBackgroundService> _logger;

    /// <summary>
    /// Initializes a new instance of the MailSenderBackgroundService
    /// </summary>
    /// <param name="serviceProvider">Service provider for dependency injection</param>
    /// <param name="configuration">Configuration for RabbitMQ connection settings</param>
    /// <param name="logger">Logger for the service</param>
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

    /// <summary>
    /// Executes the background service, processing email messages from the RabbitMQ queue
    /// </summary>
    /// <param name="stoppingToken">Token to monitor for cancellation requests</param>
    /// <returns>A task representing the background operation</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MailSenderBackgroundService started");
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var emailMessage = JsonSerializer.Deserialize<EmailMessage>(message);

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
