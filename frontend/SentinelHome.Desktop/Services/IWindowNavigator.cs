using SentinelHome.Desktop.Models;

namespace SentinelHome.Desktop.Services;

public interface IWindowNavigator
{
    void ShowMainWindow();
    void ShowLoginWindow();
    void ShowEventDetail(DashboardAlert alert, Action<DashboardAlert?> onAcknowledge);
    void ShowAlertPopup(DashboardAlert alert, Action<DashboardAlert?> onAcknowledge, Action<DashboardAlert?> onViewDetail);
}
