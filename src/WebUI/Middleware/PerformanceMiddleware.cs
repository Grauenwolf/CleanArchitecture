using System.Diagnostics;

namespace CleanArchitecture.WebUI.Middleware;

public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var timer = new Stopwatch();
        timer.Start();

        // Call the next delegate/middleware in the pipeline.
        await _next(context);

        timer.Stop();

        var elapsedMilliseconds = timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = context.Request.Path;
            var userId = context.User?.Identity?.Name;

            _logger.LogWarning($"CleanArchitecture Long Running Request: {requestName} ({elapsedMilliseconds} milliseconds) {userId}");
        }
    }
}

public static class PerformanceMiddlewareExtensions
{
    public static IApplicationBuilder UsePerformanceMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PerformanceMiddleware>();
    }
}