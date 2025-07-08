using System.Net;
using System.Text.Json;
using Serilog;

namespace consware_api.Infrastructure.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Serilog.ILogger _logger = Log.ForContext<ErrorHandlingMiddleware>();

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An unhandled exception occurred. Request: {Method} {Path}",
                context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            message = "An error occurred",
            details = exception.Message
        };

        switch (exception)
        {
            case ArgumentException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new { message = "Invalid input", details = exception.Message };
                _logger.Warning("Bad request: {Message}", exception.Message);
                break;
            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new { message = "Resource not found", details = exception.Message };
                _logger.Warning("Resource not found: {Message}", exception.Message);
                break;
            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = new { message = "Unauthorized access", details = exception.Message };
                _logger.Warning("Unauthorized access: {Message}", exception.Message);
                break;
            case InvalidOperationException:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response = new { message = "Operation not allowed", details = exception.Message };
                _logger.Warning("Invalid operation: {Message}", exception.Message);
                break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new { message = "Internal server error", details = "An unexpected error occurred" };
                _logger.Error(exception, "Internal server error");
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }
}
