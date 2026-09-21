using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SentinelHome.Desktop.Services;

namespace SentinelHome.Desktop.ViewModels;

public partial class LoginViewModel(
    IAuthService authService,
    IWindowNavigator windowNavigator,
    IFrontendMode frontendMode,
    ILogger<LoginViewModel> logger) : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _username = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _password = string.Empty;

    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string? _errorMessage;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool IsMockMode => frontendMode.UseMockData;

    partial void OnErrorMessageChanged(string? value) => OnPropertyChanged(nameof(HasError));

    public void SetPassword(string password) => Password = password;

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync(CancellationToken cancellationToken)
    {
        ErrorMessage = null;
        IsBusy = true;
        try
        {
            await authService.SignInAsync(Username, Password, cancellationToken);
            Password = string.Empty;
            windowNavigator.ShowMainWindow();
        }
        catch (AuthenticationFailedException)
        {
            ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng. Hãy thử lại.";
        }
        catch (ApiUnavailableException)
        {
            ErrorMessage = "API đang chạy nhưng PostgreSQL chưa sẵn sàng. Hãy khởi động PostgreSQL và kiểm tra ConnectionStrings:SentinelHome.";
        }
        catch (HttpRequestException exception)
        {
            logger.LogWarning(exception, "The desktop client could not reach the API while signing in.");
            ErrorMessage = "Không kết nối được API. Hãy khởi động backend rồi thử lại.";
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            ErrorMessage = null;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unexpected sign-in failure.");
            ErrorMessage = "Đăng nhập chưa thể hoàn tất. Hãy kiểm tra cấu hình backend và thử lại.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanLogin() => !IsBusy
        && !string.IsNullOrWhiteSpace(Username)
        && !string.IsNullOrWhiteSpace(Password);
}
