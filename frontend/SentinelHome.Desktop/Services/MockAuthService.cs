using SentinelHome.Contracts;

namespace SentinelHome.Desktop.Services;

/// <summary>Frontend-only authentication simulator. It never calls the API or persists credentials.</summary>
public sealed class MockAuthService : IAuthService
{
    private AuthSession? _currentSession;

    public AuthSession? CurrentSession => _currentSession is { IsExpired: false } session ? session : null;
    public string? AccessToken => CurrentSession?.AccessToken;
    public bool IsAuthenticated => CurrentSession is not null;

    public async Task<LoginResponse> SignInAsync(string username, string password, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(350), cancellationToken);

        if (string.IsNullOrWhiteSpace(username) || password != "demo123")
            throw new AuthenticationFailedException();

        var normalizedUsername = username.Trim();
        var role = string.Equals(normalizedUsername, "admin", StringComparison.OrdinalIgnoreCase)
            ? UserRole.Admin
            : UserRole.Viewer;
        var expiresAt = DateTimeOffset.UtcNow.AddHours(12);
        var token = $"mock-token-{Guid.NewGuid():N}";

        _currentSession = new AuthSession(token, expiresAt, normalizedUsername, role);
        return new LoginResponse(token, expiresAt, normalizedUsername, role);
    }

    public void SignOut() => _currentSession = null;
}
