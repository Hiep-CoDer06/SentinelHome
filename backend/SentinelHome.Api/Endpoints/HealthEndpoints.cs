using Microsoft.AspNetCore.Http.HttpResults;
using SentinelHome.Contracts;

namespace SentinelHome.Api.Endpoints;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/health", static () =>
            TypedResults.Ok(new HealthResponse("ready", DateTimeOffset.UtcNow)))
            .AllowAnonymous()
            .WithName("GetApiHealth")
            .WithSummary("Checks whether the SentinelHome API is running.")
            .Produces<HealthResponse>(StatusCodes.Status200OK);

        return app;
    }
}
