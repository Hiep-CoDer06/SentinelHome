using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SentinelHome.Desktop.Models;

namespace SentinelHome.Desktop.ViewModels;

public partial class AlertPopupViewModel : ObservableObject
{
    private Action<DashboardAlert?>? _onAcknowledge;
    private Action<DashboardAlert?>? _onViewDetail;

    [ObservableProperty] private DashboardAlert? _alert;

    public event EventHandler? RequestClose;

    public void Initialize(DashboardAlert alert, Action<DashboardAlert?> onAcknowledge, Action<DashboardAlert?> onViewDetail)
    {
        Alert = alert;
        _onAcknowledge = onAcknowledge;
        _onViewDetail = onViewDetail;
    }

    [RelayCommand]
    private void ViewDetail()
    {
        _onViewDetail?.Invoke(Alert);
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Dismiss()
    {
        _onAcknowledge?.Invoke(Alert);
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
}
