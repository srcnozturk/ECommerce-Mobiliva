using ECommerceAPI.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace ECommerceAPI.Infrastructure.Services;

public class RabbitMQService : IRabbitMQService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQService(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"],
            Port = int.Parse(configuration["RabbitMQ:Port"]),
            UserName = configuration["RabbitMQ:Username"],
            Password = configuration["RabbitMQ:Password"],
            // SSL'i devre dışı bırakıyoruz
            Ssl = new SslOption
            {
                Enabled = false
            }
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }
        catch (Exception ex)
        {
            throw new Exception($"RabbitMQ bağlantısı kurulamadı: {ex.Message}", ex);
        }
    }

    public void PublishMessage<T>(string queueName, T message)
    {
        try
        {
            _channel.QueueDeclare(queueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: body);
        }
        catch (Exception ex)
        {
            throw new Exception($"Mesaj gönderilemedi: {ex.Message}", ex);
        }
    }

    // Dispose pattern implementasyonu
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                if (_channel != null && _channel.IsOpen)
                    _channel.Close();
                if (_connection != null && _connection.IsOpen)
                    _connection.Close();
            }
            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}