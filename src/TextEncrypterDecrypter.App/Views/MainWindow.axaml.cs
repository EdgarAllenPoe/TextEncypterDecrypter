using Avalonia.Controls;
using TextEncrypterDecrypter.Core.ViewModels;

namespace TextEncrypterDecrypter.App.Views;

/// <summary>
/// Main window code-behind
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(MainViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}
