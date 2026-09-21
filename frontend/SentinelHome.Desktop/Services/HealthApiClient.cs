using System.Net.Http;
using System.Net.Http.Json;
using SentinelHome.Contracts;

namespace SentinelHome.Desktop.Services;

public interface IHealthApiClient
{
    Task<HealthResponse?> GetHealthAsync(CancellationToken cancellationToken);
}

public sealed class HealthApiClient(IHttpClientFactory httpClientFactory) : IHealthApiClient
{
    public Task<HealthResponse?> GetHealthAsync(CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient(ApiHttpClientNames.Anonymous);
        return client.GetFromJsonAsync<HealthResponse>("/api/health", ApiJson.Options, cancellationToken);
    }
}
