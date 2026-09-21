using SentinelHome.Contracts;

namespace SentinelHome.Desktop.Services;

public interface IAuthService
{
    AuthSession? CurrentSession { get; }
    string? AccessToken { get; }
    bool IsAuthenticated { get; }

    Task<LoginResponse> SignInAsync(string username, string password, CancellationToken cancellationToken);
    void SignOut();
}
