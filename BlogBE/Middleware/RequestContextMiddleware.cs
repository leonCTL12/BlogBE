using Microsoft.Extensions.Primitives;

namespace BlogBE.Middleware;

//This middleware is for enhancing activity logging by guaranteeing that each HTTP request has a unique correlation ID.
//Middleware is a chain of components that preprocess or postprocess HTTP requests (i.e. before or after the controller)
public class RequestContextMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-ID";

    //Next middleware in the pipeline
    private readonly RequestDelegate _next;

    public RequestContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        string correlationId;
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var headerValue) &&
            !StringValues.IsNullOrEmpty(headerValue))
        {
            correlationId = headerValue.ToString();
        }
        else
        {
            correlationId =
                Guid.NewGuid().ToString("n"); //n mean no formatting, i.e. 32-char string with no dashes, no braces
            context.Request.Headers[CorrelationIdHeader] = correlationId;
        }

        context.Items["CorrelationId"] = correlationId;
        context.Items["Ip"] = context.Connection.RemoteIpAddress?.ToString();
        context.Items["UserAgent"] = context.Request.Headers["User-Agent"].ToString();

        await _next(context);
    }
}