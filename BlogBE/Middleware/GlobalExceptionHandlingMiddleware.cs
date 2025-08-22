using BlogBE.Constants;
using Microsoft.AspNetCore.Mvc;

namespace BlogBE.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
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
        //In theory, corelation Id should be set in RequestContextMiddleware already
        var correlationId = context.Items.TryGetValue("CorrelationId", out var corelationIdObj)
            ? corelationIdObj.ToString()
            : Guid.NewGuid().ToString("n");

        _logger.LogError(exception, "An unhandled exception occurred. CorrelationId: {CorrelationId}", correlationId);

        var (statusCode, title) = exception switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, GlobalExceptionConstants.NotFound404Title),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized,
                GlobalExceptionConstants.Unauthorized401Title),
            ArgumentException => (StatusCodes.Status400BadRequest, GlobalExceptionConstants.BadRequest400Title),
            _ => (StatusCodes.Status500InternalServerError, GlobalExceptionConstants.InternalServerError500Title)
        };

        var problemDetails = new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{statusCode}",
            Title = title,
            Status = statusCode,
            Detail = _environment.IsDevelopment()
                ? exception.ToString()
                : "Please contact support with the correlation ID.",
            Instance = context.Request.Path
        };

        problemDetails.Extensions["correlationId"] = correlationId;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}