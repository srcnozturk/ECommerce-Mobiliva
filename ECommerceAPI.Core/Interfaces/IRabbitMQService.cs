namespace ECommerceAPI.Core.Interfaces;

/// <summary>
/// Interface for RabbitMQ messaging service operations.
/// Provides methods for publishing messages to RabbitMQ queues.
/// </summary>
public interface IRabbitMQService : IAsyncDisposable
{
    /// <summary>
    /// Publishes a message to a specified RabbitMQ queue asynchronously
    /// </summary>
    /// <typeparam name="T">The type of message to publish</typeparam>
    /// <param name="queueName">Name of the queue to publish to</param>
    /// <param name="message">The message to publish</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task PublishMessageAsync<T>(string queueName, T message);
}
