using System.Net.Http.Headers;
using System.Net.Http;

namespace SentinelHome.Desktop.Services;

public sealed class AuthorizationHeaderHandler(IAuthService authService) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (authService.AccessToken is { Length: > 0 } accessToken)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return base.SendAsync(request, cancellationToken);
    }
}
