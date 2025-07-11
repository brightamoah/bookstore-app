using System.Net;
using System.Text.Json;
using Backend.Models;

namespace Backend.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred. TraceId: {TraceId}", context.TraceIdentifier);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            TraceId = context.TraceIdentifier,
            Timestamp = DateTime.UtcNow
        };

        switch (exception)
        {
            case ArgumentException argEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Invalid request data provided";
                errorResponse.Details = argEx.Message;
                errorResponse.ErrorCode = ErrorCodes.INVALID_FORMAT;
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "Access denied. Please check your credentials";
                errorResponse.ErrorCode = ErrorCodes.UNAUTHORIZED;
                break;

            case FileNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = "Requested resource not found";
                errorResponse.ErrorCode = ErrorCodes.BOOK_NOT_FOUND;
                break;

            case InvalidOperationException invOpEx when invOpEx.Message.Contains("database"):
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "Database operation failed. Please try again later";
                errorResponse.ErrorCode = ErrorCodes.DATABASE_ERROR;
                break;

            case InvalidOperationException invOpEx when invOpEx.Message.Contains("image"):
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Image processing failed";
                errorResponse.Details = invOpEx.Message;
                errorResponse.ErrorCode = ErrorCodes.IMAGE_PROCESSING_FAILED;
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "An internal server error occurred. Please try again later";
                errorResponse.ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR;
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}



