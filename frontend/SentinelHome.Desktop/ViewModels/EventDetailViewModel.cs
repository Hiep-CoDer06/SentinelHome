using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SentinelHome.Desktop.Models;

namespace SentinelHome.Desktop.ViewModels;

public partial class EventDetailViewModel : ObservableObject
{
    private Action<DashboardAlert?>? _onAcknowledge;

    [ObservableProperty] private DashboardAlert? _alert;

    public event EventHandler? RequestClose;

    public void Initialize(DashboardAlert alert, Action<DashboardAlert?> onAcknowledge)
    {
        Alert = alert;
        _onAcknowledge = onAcknowledge;
    }

    [RelayCommand]
    private void Acknowledge()
    {
        _onAcknowledge?.Invoke(Alert);
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Close() => RequestClose?.Invoke(this, EventArgs.Empty);
}
