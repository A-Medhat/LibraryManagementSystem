using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using LibraryManagementSystem.Common.Exceptions;

namespace LibraryManagementSystem.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred.";

        switch (exception)
        {
            case NotFoundException:

                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
                break;

            case ConflictException:
                statusCode = HttpStatusCode.Conflict;
                message = exception.Message;
                break;

            case ValidationException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
                
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                message = exception.Message;
                break;
        }

        context.Response.StatusCode = (int)statusCode;

        var result = JsonSerializer.Serialize(new { message });
        return context.Response.WriteAsync(result);
    }
}
