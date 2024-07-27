using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;

namespace BetterCodeBox.Desktop;

public class BlazorWebView : NativeControlHost
{
    private Uri? _source = null;
    private Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView? _blazorWebView;
    private string? _hostPage;
    private IServiceProvider _serviceProvider = default!;
    private RootComponentsCollection _rootComponents = new();

    public static readonly DirectProperty<BlazorWebView, IServiceProvider> ServicesProperty
        = AvaloniaProperty.RegisterDirect<BlazorWebView, IServiceProvider>(
            nameof(Services),
            x => x.Services,
            (x, y) => x.Services = y);

    public static readonly DirectProperty<BlazorWebView, RootComponentsCollection> RootComponentsProperty
        = AvaloniaProperty.RegisterDirect<BlazorWebView, RootComponentsCollection>(
            nameof(RootComponents),
            x => x.RootComponents,
            (x, y) => x.RootComponents = y);

    public Uri? Source
    {
        get
        {
            if(_blazorWebView != null)
            {
                _source = _blazorWebView.WebView.Source;
            }
            return _source;
        }

        set
        {
            if(_source != value)
            {
                _source = value;
                if(_blazorWebView != null)
                {
                    _blazorWebView.WebView.Source = value;
                }
            }
        }
    }

    public IServiceProvider Services
    {
        get
        {
            return _serviceProvider;
        }
        set
        {
            _serviceProvider = value;
            if(_blazorWebView != null)
            {
                _blazorWebView.Services = _serviceProvider;
            }
        }
    }

    public RootComponentsCollection RootComponents
    {
        get
        {
            return _rootComponents;
        }
        set
        {
            _rootComponents = value;
        }
    }

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        if(OperatingSystem.IsWindows())
        {
            _blazorWebView = new()
            {
                HostPage = "wwwroot\\index.html",
                Services = _serviceProvider,
            };

            foreach(var component in _rootComponents)
            {
                _blazorWebView.RootComponents.Add(component);
            }

            return new PlatformHandle(_blazorWebView.Handle, "HWND");
        }

        return base.CreateNativeControlCore(parent);
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        if(OperatingSystem.IsWindows())
        {
            _blazorWebView?.Dispose();
            _blazorWebView = null;
        }
        else
        {
            base.DestroyNativeControlCore(control);
        }
    }

    // DestroyNativeControlCore doesn't seem to get called when the app is shutting down, so lets dispose the blazorwebview earlier..
    protected override void OnUnloaded(RoutedEventArgs e)
    {
        if(OperatingSystem.IsWindows())
        {
            _blazorWebView?.Dispose();
            _blazorWebView = null;
        }
        base.OnUnloaded(e);
    }
}
