using SentinelHome.Contracts;

namespace SentinelHome.Desktop.Services;

/// <summary>Frontend-only health source used while Task 2 and PostgreSQL are not available.</summary>
public sealed class MockHealthApiClient : IHealthApiClient
{
    public async Task<HealthResponse?> GetHealthAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken);
        return new HealthResponse("mock-ready", DateTimeOffset.UtcNow);
    }
}
