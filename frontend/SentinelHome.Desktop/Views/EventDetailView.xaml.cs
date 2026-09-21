using System.Windows;
using SentinelHome.Desktop.ViewModels;

namespace SentinelHome.Desktop.Views;

public partial class EventDetailView : Window
{
    public EventDetailView(EventDetailViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.RequestClose += (_, _) => Close();
    }
}
