namespace SentinelHome.Api.Configuration;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "SentinelHome.Api";
    public string Audience { get; init; } = "SentinelHome.Desktop";
    public string Key { get; init; } = string.Empty;
    public int ExpirationHours { get; init; } = 8;
}
