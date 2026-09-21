namespace SentinelHome.Desktop.Services;

public sealed class AuthenticationFailedException : Exception
{
    public AuthenticationFailedException()
        : base("Tên đăng nhập hoặc mật khẩu không đúng.")
    {
    }
}
