using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TextEncrypterDecrypter.Core.Services;
using TextEncrypterDecrypter.Core.ViewModels;
using TextEncrypterDecrypter.App.Views;

namespace TextEncrypterDecrypter.App;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        var host = CreateHostBuilder().Build();
        
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .AfterSetup(builder =>
            {
                // Configure the main window with dependency injection
                if (builder.Instance?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.MainWindow = host.Services.GetRequiredService<MainWindow>();
                }
            });
    }

    private static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Register services
                services.AddSingleton<IEncryptionService, EncryptionService>();
                services.AddSingleton<ISettingsService, JsonSettingsService>();
                
                // Register ViewModels
                services.AddTransient<MainViewModel>();
                
                // Register Views
                services.AddTransient<MainWindow>();
            });
}
