using Avalonia.ReactiveUI;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using ReactiveUI;
using System.Reactive;
using BetterCodeBox.Desktop.ViewModels;

namespace BetterCodeBox.Desktop.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        var rootComponents = new RootComponentsCollection()
        {
            new RootComponent("#app", typeof(BetterCodeBox.Desktop.Main), null)
        };

        Resources.Add("services", App.AppHost!.Services);
        Resources.Add("rootComponents", rootComponents);

        InitializeComponent();
    
        this.WhenActivated(d => d(ViewModel!.ExitInteraction.RegisterHandler(DoExitAsync)));
    }

    private async Task DoExitAsync(InteractionContext<Unit, Unit> ic)
    {
        Close();
        await Task.CompletedTask;
        ic.SetOutput(Unit.Default);
    }
}
