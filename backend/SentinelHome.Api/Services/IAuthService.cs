using SentinelHome.Contracts;

namespace SentinelHome.Api.Services;

public interface IAuthService
{
    Task<LoginResponse?> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken);
}
