namespace ECommerceAPI.Core;

public static class ErrorCodes
{
    // HTTP Status Code Based Errors
    public const string NotFound = "404";
    public const string ValidationError = "400";
    public const string ServerError = "500";
    public const string CacheError = "501";
    public const string DatabaseError = "502";
    public const string AuthenticationError = "401";
    public const string AuthorizationError = "403";

    // Product Related Errors
    public const string ProductRetrievalError = "PRD001";
    public const string ProductNotFoundError = "PRD002";
    public const string ProductValidationError = "PRD003";
    public const string ProductCreationError = "PRD004";
    public const string ProductUpdateError = "PRD005";
    public const string ProductDeletionError = "PRD006";

    // Order Related Errors
    public const string OrderCreationError = "ORD001";
    public const string OrderNotFoundError = "ORD002";
    public const string OrderProcessingError = "ORD003";

    // Email Related Errors
    public const string EmailSendingError = "EML001";
    public const string EmailValidationError = "EML002";

    // Cache Related Errors
    public const string CacheOperationError = "CCH001";
    public const string CacheConnectionError = "CCH002";

    // RabbitMQ Related Errors
    public const string MessageQueueError = "RMQ001";
    public const string MessagePublishError = "RMQ002";
    public const string MessageConsumeError = "RMQ003";
}
