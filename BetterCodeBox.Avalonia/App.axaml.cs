using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BetterCodeBox.Desktop.Data;
using BetterCodeBox.Desktop.ViewModels;
using BetterCodeBox.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BetterCodeBox.Desktop;

public partial class App : Application
{
    public static IHost? AppHost { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }
        base.OnFrameworkInitializationCompleted();
    }

    internal static void RunAvaloniaAppWithHosting(string[] args, Func<AppBuilder> buildAvaloniaApp)
    {
        var appBuilder = Host.CreateApplicationBuilder(args);
        appBuilder.Logging.AddDebug();
        appBuilder.Services.AddWindowsFormsBlazorWebView();
        appBuilder.Services.AddBlazorWebViewDeveloperTools();
        appBuilder.Services.AddSingleton<AvaloniaFilePickerService>();
        using var myApp = appBuilder.Build();
        App.AppHost = myApp;

        myApp.Start();

        try
        {
            buildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }
        finally
        {
            Task.Run(async () => await myApp.StopAsync()).GetAwaiter().GetResult();
        }
    }
}
