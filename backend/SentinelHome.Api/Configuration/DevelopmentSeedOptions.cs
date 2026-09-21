namespace SentinelHome.Api.Configuration;

/// <summary>Development-only first-user settings. Do not enable these credentials in production.</summary>
public sealed class DevelopmentSeedOptions
{
    public const string SectionName = "DevelopmentSeed";

    public string Username { get; init; } = "admin";
    public string Password { get; init; } = string.Empty;
}
