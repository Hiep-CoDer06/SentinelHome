using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SentinelHome.Api.Configuration;
using SentinelHome.Contracts;
using SentinelHome.Data;

namespace SentinelHome.Api.Services;

/// <summary>Creates the local development schema and its first administrator only in Development.</summary>
public sealed class DevelopmentDatabaseInitializer(
    SentinelHomeDbContext dbContext,
    IOptions<DevelopmentSeedOptions> seedOptions,
    ILogger<DevelopmentDatabaseInitializer> logger)
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);

            if (await dbContext.Users.AnyAsync(cancellationToken))
                return;

            var seed = seedOptions.Value;
            if (string.IsNullOrWhiteSpace(seed.Username) || string.IsNullOrWhiteSpace(seed.Password))
            {
                logger.LogWarning("No development administrator was seeded because DevelopmentSeed is incomplete.");
                return;
            }

            dbContext.Users.Add(new User
            {
                Username = seed.Username.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(seed.Password),
                Role = UserRole.Admin,
                CreatedAt = DateTimeOffset.UtcNow
            });
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Created the development administrator {Username}.", seed.Username);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogWarning(exception, "The development database is unavailable. Health checks remain available, but sign-in requires PostgreSQL.");
        }
    }
}
