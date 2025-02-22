namespace ECommerceAPI.Core.Interfaces;

public interface IRabbitMQService : IAsyncDisposable
{
    Task PublishMessageAsync<T>(string queueName, T message);
}
