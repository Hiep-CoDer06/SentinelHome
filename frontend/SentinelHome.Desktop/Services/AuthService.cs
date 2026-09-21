using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using SentinelHome.Contracts;

namespace SentinelHome.Desktop.Services;

public sealed class AuthService(IHttpClientFactory httpClientFactory) : IAuthService
{
    private AuthSession? _currentSession;

    public AuthSession? CurrentSession => _currentSession is { IsExpired: false } session ? session : null;
    public string? AccessToken => CurrentSession?.AccessToken;
    public bool IsAuthenticated => CurrentSession is not null;

    public async Task<LoginResponse> SignInAsync(
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient(ApiHttpClientNames.Anonymous);
        using var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest(username.Trim(), password),
            ApiJson.Options,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new AuthenticationFailedException();
        if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            throw new ApiUnavailableException();

        response.EnsureSuccessStatusCode();
        var login = await response.Content.ReadFromJsonAsync<LoginResponse>(ApiJson.Options, cancellationToken)
            ?? throw new InvalidOperationException("API không trả về dữ liệu phiên đăng nhập.");

        _currentSession = new AuthSession(login.AccessToken, login.ExpiresAt, login.Username, login.Role);
        return login;
    }

    public void SignOut() => _currentSession = null;
}
