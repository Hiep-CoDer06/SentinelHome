using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SentinelHome.Desktop.Services;
using SentinelHome.Desktop.ViewModels;
using SentinelHome.Desktop.Views;

namespace SentinelHome.Desktop;

public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(configuration =>
            {
                configuration.SetBasePath(AppContext.BaseDirectory);
                configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            })
            .ConfigureServices((context, services) =>
            {
                var baseUrl = context.Configuration["Api:BaseUrl"] ?? "http://localhost:5081";
                var apiUri = new Uri($"{baseUrl.TrimEnd('/')}/");
                var useMockData = bool.TryParse(context.Configuration["Api:UseMockData"], out var configuredMockMode)
                    && configuredMockMode;
                services.AddHttpClient(ApiHttpClientNames.Anonymous, client => client.BaseAddress = apiUri);
                services.AddTransient<AuthorizationHeaderHandler>();
                services.AddHttpClient(ApiHttpClientNames.Authorized, client => client.BaseAddress = apiUri)
                    .AddHttpMessageHandler<AuthorizationHeaderHandler>();
                services.AddSingleton<IFrontendMode>(new FrontendMode(useMockData));
                if (useMockData)
                {
                    services.AddSingleton<IAuthService, MockAuthService>();
                    services.AddSingleton<IHealthApiClient, MockHealthApiClient>();
                }
                else
                {
                    services.AddSingleton<IAuthService, AuthService>();
                    services.AddSingleton<IHealthApiClient, HealthApiClient>();
                }
                services.AddSingleton<IWindowNavigator, WindowNavigator>();
                services.AddTransient<LoginViewModel>();
                services.AddTransient<LoginView>();
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>();
                services.AddTransient<EventDetailViewModel>();
                services.AddTransient<EventDetailView>();
                services.AddTransient<AlertPopupViewModel>();
                services.AddTransient<AlertPopupWindow>();
            })
            .Build();
        DispatcherUnhandledException += OnDispatcherUnhandledException;
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        await _host.StartAsync();
        _host.Services.GetRequiredService<IWindowNavigator>().ShowLoginWindow();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        base.OnExit(e);
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var logger = _host.Services.GetRequiredService<ILogger<App>>();
        logger.LogError(e.Exception, "Unhandled desktop UI exception");
        MessageBox.Show("Đã xảy ra lỗi không mong muốn. Hãy kiểm tra log và thử lại.", "SentinelHome", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }
}
