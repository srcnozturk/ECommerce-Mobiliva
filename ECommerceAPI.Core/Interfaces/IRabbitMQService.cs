namespace ECommerceAPI.Core.Interfaces;

public interface IRabbitMQService
{
    void PublishMessage<T>(string queueName, T message);
}
