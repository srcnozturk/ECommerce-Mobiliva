namespace ECommerceAPI.Core;

public class ApiException : Exception
{
    public string ErrorCode { get; }

    public ApiException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}
