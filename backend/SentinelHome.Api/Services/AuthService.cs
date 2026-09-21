using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using SentinelHome.Api.Configuration;
using SentinelHome.Contracts;
using SentinelHome.Data;

namespace SentinelHome.Api.Services;

public sealed class AuthService(
    SentinelHomeDbContext dbContext,
    IOptions<JwtOptions> jwtOptions,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<LoginResponse?> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return null;

            var username = request.Username.Trim();
            var user = await dbContext.Users
                .SingleOrDefaultAsync(x => x.Username == username, cancellationToken);

            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                logger.LogWarning("A failed login attempt was made for username {Username}.", username);
                return null;
            }

            var options = jwtOptions.Value;
            if (string.IsNullOrWhiteSpace(options.Key))
                throw new InvalidOperationException("JWT signing key is not configured.");

            var expiresAt = DateTimeOffset.UtcNow.AddHours(Math.Max(1, options.ExpirationHours));
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)),
                SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            var token = new JwtSecurityToken(
                options.Issuer,
                options.Audience,
                claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt.UtcDateTime,
                signingCredentials: credentials);

            user.LastLoginAt = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("User {Username} signed in.", user.Username);

            return new LoginResponse(
                new JwtSecurityTokenHandler().WriteToken(token),
                expiresAt,
                user.Username,
                user.Role);
        }
        catch (NpgsqlException exception)
        {
            throw new DatabaseUnavailableException(exception);
        }
        catch (InvalidOperationException exception) when (exception.InnerException is NpgsqlException)
        {
            throw new DatabaseUnavailableException(exception);
        }
    }
}
