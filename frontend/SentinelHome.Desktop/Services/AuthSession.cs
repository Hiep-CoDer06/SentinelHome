using SentinelHome.Contracts;

namespace SentinelHome.Desktop.Services;

public sealed record AuthSession(string AccessToken, DateTimeOffset ExpiresAt, string Username, UserRole Role)
{
    public bool IsExpired => ExpiresAt <= DateTimeOffset.UtcNow;
}
