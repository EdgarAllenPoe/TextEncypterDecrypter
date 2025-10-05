using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TextEncrypterDecrypter.Core.Services;
using TextEncrypterDecrypter.Core.ViewModels;
using TextEncrypterDecrypter.App.Views;
using TextEncrypterDecrypter.App.Services;

namespace TextEncrypterDecrypter.App;

public partial class App : Application
{
    private static IHost? _host;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Create host and services
            _host = CreateHostBuilder().Build();
            
            // Create MainWindow manually with ViewModel
            var viewModel = _host.Services.GetRequiredService<MainViewModel>();
            var mainWindow = new Views.MainWindow(viewModel);
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Register services
                services.AddSingleton<IEncryptionService, EncryptionService>();
                services.AddSingleton<ISettingsService, JsonSettingsService>();
                services.AddSingleton<IClipboardService, Services.ClipboardService>();
                
                // Register ViewModels
                services.AddTransient<MainViewModel>();
                
                // Register Views
                services.AddTransient<Views.MainWindow>();
            });
}