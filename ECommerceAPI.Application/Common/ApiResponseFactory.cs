using ECommerceAPI.Core;

namespace ECommerceAPI.Application;

public static class ApiResponseFactory
{
    public static ApiResponse<T> Success<T>(T data, string message = "Operation completed successfully")
    {
        return new ApiResponse<T>
        {
            Status = Status.Success,
            ResultMessage = message,
            Data = data,
            ErrorCode = null
        };
    }

    public static ApiResponse<T> Error<T>(string message, string errorCode, T data = default)
    {
        return new ApiResponse<T>
        {
            Status = Status.Failed,
            ResultMessage = message,
            ErrorCode = errorCode,
            Data = data
        };
    }

    public static ApiResponse<T> NotFound<T>(string message = "Resource not found")
    {
        return Error<T>(message, ErrorCodes.NotFound);
    }

    public static ApiResponse<T> ValidationError<T>(string message = "Validation failed")
    {
        return Error<T>(message, ErrorCodes.ValidationError);
    }

    public static ApiResponse<T> ServerError<T>(string message = "An internal server error occurred")
    {
        return Error<T>(message, ErrorCodes.ServerError);
    }

    public static ApiResponse<T> ProductError<T>(string message, string errorCode = ErrorCodes.ProductRetrievalError)
    {
        return Error<T>(message, errorCode);
    }

    public static ApiResponse<T> OrderError<T>(string message, string errorCode = ErrorCodes.OrderProcessingError)
    {
        return Error<T>(message, errorCode);
    }

    public static ApiResponse<T> EmailError<T>(string message, string errorCode = ErrorCodes.EmailSendingError)
    {
        return Error<T>(message, errorCode);
    }

    public static ApiResponse<T> CacheError<T>(string message, string errorCode = ErrorCodes.CacheOperationError)
    {
        return Error<T>(message, errorCode);
    }

    public static ApiResponse<T> MessageQueueError<T>(string message, string errorCode = ErrorCodes.MessageQueueError)
    {
        return Error<T>(message, errorCode);
    }

    public static T Failure<T>(string message, string errorCode)
    {
        throw new NotImplementedException();
    }
}
