namespace ECommerceAPI.Core;

public class ApiResponse<T>
{
    public Status Status { get; set; }
    public string ResultMessage { get; set; }
    public string ErrorCode { get; set; }
    public T Data { get; set; }
}

public enum Status
{
    Success = 1,
    Failed = 2
}