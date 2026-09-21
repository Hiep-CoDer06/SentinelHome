namespace SentinelHome.Desktop.Services;

public sealed class FrontendMode(bool useMockData) : IFrontendMode
{
    public bool UseMockData { get; } = useMockData;
}
