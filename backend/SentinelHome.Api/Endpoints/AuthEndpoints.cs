using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SentinelHome.Api.Services;
using SentinelHome.Contracts;

namespace SentinelHome.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/login", LoginAsync)
            .AllowAnonymous()
            .WithName("Login")
            .WithSummary("Authenticates a desktop user and returns a JWT access token.")
            .WithDescription("The password is verified on the server with BCrypt. Invalid credentials never reveal which field was incorrect.")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status503ServiceUnavailable);

        return app;
    }

    private static async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult, BadRequest>> LoginAsync(
        LoginRequest request,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return TypedResults.BadRequest();

        var response = await authService.AuthenticateAsync(request, cancellationToken);
        return response is null ? TypedResults.Unauthorized() : TypedResults.Ok(response);
    }
}
