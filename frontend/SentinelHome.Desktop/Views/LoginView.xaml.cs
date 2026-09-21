using System.Windows;
using System.Windows.Controls;
using SentinelHome.Desktop.ViewModels;

namespace SentinelHome.Desktop.Views;

public partial class LoginView : Window
{
    public LoginView(LoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void PasswordInput_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel viewModel && sender is PasswordBox passwordBox)
            viewModel.SetPassword(passwordBox.Password);
    }
}
