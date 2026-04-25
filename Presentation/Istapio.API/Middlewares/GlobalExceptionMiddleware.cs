// API/Middleware/GlobalExceptionMiddleware.cs
using Istapio.Application.Exceptions;
using Istapio.Application.Models.Responses;
using System.Net;
using System.Text.Json;

namespace Istapio.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        LogException(context, exception);

        var (statusCode, title) = GetErrorInfo(exception);

        ErrorDetails errorResponse;

        if (exception is ValidationException validationEx)
        {
            errorResponse = new ValidationErrorResponse
            {
                Type = $"https://httpstatuses.com/{(int)statusCode}",
                Title = title,
                Status = (int)statusCode,
                Detail = validationEx.Message,
                Instance = context.Request.Path,
                TraceId = context.TraceIdentifier,
                ValidationErrors = validationEx.Errors
            };
        }
        else
        {
            errorResponse = new ErrorDetails
            {
                Type = $"https://httpstatuses.com/{(int)statusCode}",
                Title = title,
                Status = (int)statusCode,
                Detail = GetErrorDetail(exception),
                Instance = context.Request.Path,
                TraceId = context.TraceIdentifier
            };

            if (_env.IsDevelopment())
            {
                errorResponse.StackTrace = exception.StackTrace;
                errorResponse.InnerException = exception.InnerException?.Message;
            }
        }

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _env.IsDevelopment(),
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        await context.Response.WriteAsync(json);
    }

    private (HttpStatusCode statusCode, string title) GetErrorInfo(Exception exception)
    {
        return exception switch
        {
            // 400 Bad Request
            ArgumentException => (HttpStatusCode.BadRequest, "Bad Request"),
            InvalidOperationException => (HttpStatusCode.BadRequest, "Invalid Operation"),

            // 401 Unauthorized
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized"),
            UnauthorizedException => (HttpStatusCode.Unauthorized, "Unauthorized"),

            // 403 Forbidden
            ForbiddenException => (HttpStatusCode.Forbidden, "Forbidden"),

            // 404 Not Found
            NotFoundException => (HttpStatusCode.NotFound, "Resource Not Found"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Key Not Found"),

            // 409 Conflict
            ConflictException => (HttpStatusCode.Conflict, "Conflict"),

            // 422 Unprocessable Entity
            ValidationException => (HttpStatusCode.UnprocessableEntity, "Validation Failed"),

            // 429 Too Many Requests
            TooManyRequestsException => (HttpStatusCode.TooManyRequests, "Too Many Requests"),

            // 500 Internal Server Error
            _ => (HttpStatusCode.InternalServerError, "Internal Server Error")
        };
    }

    private string GetErrorDetail(Exception exception)
    {
        if (!_env.IsDevelopment() && exception is not BaseException)
        {
            return "An unexpected error occurred. Please try again later.";
        }

        return exception.Message;
    }

    private void LogException(HttpContext context, Exception exception)
    {
        var logLevel = exception switch
        {
            ArgumentException => LogLevel.Warning,
            NotFoundException => LogLevel.Warning,
            ValidationException => LogLevel.Warning,
            UnauthorizedException => LogLevel.Information,
            ForbiddenException => LogLevel.Warning,
            ConflictException => LogLevel.Warning,
            _ => LogLevel.Error
        };

        _logger.Log(logLevel, exception,
            "Error | User: {User} | IP: {IP} | Path: {Path} | Method: {Method} | TraceId: {TraceId}",
            context.User.Identity?.Name ?? "Anonymous",
            context.Connection.RemoteIpAddress,
            context.Request.Path,
            context.Request.Method,
            context.TraceIdentifier
        );
    }
}