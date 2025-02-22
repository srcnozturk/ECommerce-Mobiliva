using ECommerceAPI.Application;
using ECommerceAPI.Core;
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = exception switch
        {
            ApiException apiException =>
                ApiResponseFactory.Failure<object>(apiException.Message, apiException.ErrorCode),

            KeyNotFoundException =>
                ApiResponseFactory.Failure<object>("Resource not found", ErrorCodes.NotFound),

            ValidationException validationException =>
                ApiResponseFactory.Failure<object>(validationException.Message, ErrorCodes.ValidationError),

            _ => ApiResponseFactory.Failure<object>(
                "An internal server error occurred.",
                ErrorCodes.ServerError)
        };

        var errorCode = response.GetType().GetProperty("ErrorCode")?.GetValue(response)?.ToString();
        context.Response.StatusCode = int.TryParse(errorCode, out var statusCode) ? statusCode : 500;
        await context.Response.WriteAsJsonAsync(response);
    }
}
