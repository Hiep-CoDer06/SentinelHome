using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SentinelHome.Desktop.Models;
using SentinelHome.Desktop.ViewModels;
using SentinelHome.Desktop.Views;

namespace SentinelHome.Desktop.Services;

public sealed class WindowNavigator(IServiceProvider serviceProvider) : IWindowNavigator
{
    public void ShowMainWindow()
    {
        var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();

        foreach (var loginView in Application.Current.Windows.OfType<LoginView>().ToArray())
            loginView.Close();
    }

    public void ShowLoginWindow()
    {
        foreach (var mainWindow in Application.Current.Windows.OfType<MainWindow>())
            mainWindow.Hide();

        serviceProvider.GetRequiredService<LoginView>().Show();
    }

    public void ShowEventDetail(DashboardAlert alert, Action<DashboardAlert?> onAcknowledge)
    {
        var view = serviceProvider.GetRequiredService<EventDetailView>();
        ((EventDetailViewModel)view.DataContext).Initialize(alert, onAcknowledge);
        view.Owner = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        view.ShowDialog();
    }

    public void ShowAlertPopup(DashboardAlert alert, Action<DashboardAlert?> onAcknowledge, Action<DashboardAlert?> onViewDetail)
    {
        var window = serviceProvider.GetRequiredService<AlertPopupWindow>();
        ((AlertPopupViewModel)window.DataContext).Initialize(alert, onAcknowledge, onViewDetail);
        window.Show();
    }
}
