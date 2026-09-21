namespace SentinelHome.Desktop.Services;

public sealed class ApiUnavailableException : Exception
{
    public ApiUnavailableException()
        : base("The SentinelHome API cannot access its database.")
    {
    }
}
