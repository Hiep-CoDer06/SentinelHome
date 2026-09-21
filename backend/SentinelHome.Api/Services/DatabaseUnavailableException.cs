namespace SentinelHome.Api.Services;

public sealed class DatabaseUnavailableException : Exception
{
    public DatabaseUnavailableException(Exception innerException)
        : base("The SentinelHome database is unavailable.", innerException)
    {
    }
}
