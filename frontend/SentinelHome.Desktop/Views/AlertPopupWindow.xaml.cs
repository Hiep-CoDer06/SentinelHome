using System.Windows;
using System.Windows.Threading;
using SentinelHome.Desktop.ViewModels;

namespace SentinelHome.Desktop.Views;

public partial class AlertPopupWindow : Window
{
    private readonly DispatcherTimer _autoHideTimer = new() { Interval = TimeSpan.FromSeconds(8) };

    public AlertPopupWindow(AlertPopupViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.RequestClose += (_, _) => Close();

        var workArea = SystemParameters.WorkArea;
        Left = workArea.Right - Width - 24;
        Top = workArea.Bottom - Height - 24;

        _autoHideTimer.Tick += (_, _) =>
        {
            _autoHideTimer.Stop();
            Close();
        };
        Loaded += (_, _) => _autoHideTimer.Start();
        Closed += (_, _) => _autoHideTimer.Stop();
    }
}
