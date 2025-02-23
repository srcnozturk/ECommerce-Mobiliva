using RabbitMQ.Client.Exceptions;

namespace ECommerceAPI.Infrastructure.Services;

public class RabbitMQService : IRabbitMQService, IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQService> _logger;
    private bool _disposed;

    public RabbitMQService(IConfiguration configuration, ILogger<RabbitMQService> logger)
    {
        _logger = logger;
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:Host"],
                Port = int.Parse(configuration["RabbitMQ:Port"]),
                UserName = configuration["RabbitMQ:Username"],
                Password = configuration["RabbitMQ:Password"],
                Ssl = new SslOption { Enabled = false }
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _logger.LogInformation("RabbitMQ connection established successfully");
        }
        catch (BrokerUnreachableException ex)
        {
            _logger.LogError(ex, "RabbitMQ broker is unreachable");
            throw new Exception("RabbitMQ service is currently unavailable", ex);
        }
        catch (AuthenticationFailureException ex)
        {
            _logger.LogError(ex, "RabbitMQ authentication failed");
            throw new Exception("Failed to authenticate with RabbitMQ service", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ connection");
            throw new Exception("Failed to initialize message queue service", ex);
        }
    }

    public async Task PublishMessageAsync<T>(string queueName, T message)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RabbitMQService));
        }

        try
        {
            await Task.Run(() =>
            {
                _channel.QueueDeclare(queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                _channel.BasicPublish(
                    exchange: "",
                    routingKey: queueName,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation("Message published successfully to queue: {QueueName}", queueName);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to queue: {QueueName}", queueName);
            throw new Exception($"Failed to publish message to queue: {queueName}", ex);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        if (_channel?.IsOpen == true)
        {
            await Task.Run(() => _channel.Close());
        }

        if (_connection?.IsOpen == true)
        {
            await Task.Run(() => _connection.Close());
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}