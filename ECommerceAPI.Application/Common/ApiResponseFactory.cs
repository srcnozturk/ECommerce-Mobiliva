using ECommerceAPI.Core;

namespace ECommerceAPI.Application;

public class ApiResponseFactory
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

    public static ApiResponse<T> Failure<T>(string message, string errorCode, T data = default)
    {
        return new ApiResponse<T>
        {
            Status = Status.Failed,
            ResultMessage = message,
            ErrorCode = errorCode,
            Data = data
        };
    }
}
