using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace BugBucks.Shared.Web.Middleware;

public sealed class RequestLoggingMiddleware : IMiddleware
{
    private readonly ILogger _logger = Log.ForContext<RequestLoggingMiddleware>();

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var sw = Stopwatch.StartNew();
        await next(context);
        sw.Stop();

        var cid = context.Items["X-Correlation-Id"] as string;
        _logger.Information(
            "[{Method}] {Path} → {StatusCode} in {Elapsed} ms (cid:{CorrelationId})",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            sw.ElapsedMilliseconds,
            cid
        );
    }
}