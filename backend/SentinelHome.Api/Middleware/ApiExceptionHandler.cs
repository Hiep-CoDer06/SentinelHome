using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SentinelHome.Api.Services;

namespace SentinelHome.Api.Middleware;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DatabaseUnavailableException)
            return false;

        logger.LogError(exception, "A request could not access the SentinelHome database.");
        httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status503ServiceUnavailable,
            Title = "Database unavailable",
            Detail = "The SentinelHome database is not available. Start PostgreSQL and verify the connection string.",
            Instance = httpContext.Request.Path
        }, cancellationToken);
        return true;
    }
}
