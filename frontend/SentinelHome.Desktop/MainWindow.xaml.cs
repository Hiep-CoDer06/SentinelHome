using System.Windows;
using SentinelHome.Desktop.ViewModels;

namespace SentinelHome.Desktop;

public partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; }

    public MainWindow(MainViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
        DataContext = ViewModel;
        ViewModel.LoadSession();
    }

    private async void Dashboard_OnLoaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.RefreshApiStatusCommand.ExecuteAsync(null);
    }
}
