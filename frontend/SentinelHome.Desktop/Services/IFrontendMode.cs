namespace SentinelHome.Desktop.Services;

/// <summary>Exposes whether the desktop client is using local mock data or the real API.</summary>
public interface IFrontendMode
{
    bool UseMockData { get; }
}
