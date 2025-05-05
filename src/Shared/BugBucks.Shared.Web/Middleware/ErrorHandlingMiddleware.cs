using System.Text.Json;
using BugBucks.Shared.Logging.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BugBucks.Shared.Web.Middleware;

public sealed class ErrorHandlingMiddleware : IMiddleware
{
    private readonly IAppLogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(IAppLogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            ctx.Response.ContentType = "application/json";

            var problem = new ProblemDetails
            {
                Status = 500,
                Title = "Internal Server Error",
                Detail = ex.Message,
                Extensions = { ["traceId"] = ctx.TraceIdentifier }
            };

            await ctx.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}