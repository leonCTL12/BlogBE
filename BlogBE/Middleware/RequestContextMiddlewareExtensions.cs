namespace BlogBE.Middleware;

public static class RequestContextMiddlewareExtensions
{
    public static void UseRequestContext(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<RequestContextMiddleware>();
    }
}