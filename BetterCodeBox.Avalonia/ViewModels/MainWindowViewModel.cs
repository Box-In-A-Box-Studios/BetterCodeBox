using System;
using ReactiveUI;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.ComponentModel.DataAnnotations;
using Avalonia.Threading;
using BetterCodeBox.Desktop.Data;
using BetterCodeBox.Desktop.ViewModels;
using BetterCodeBox.Lib;
using BetterCodeBox.Lib.Interfaces;
using BetterCodeBox.RazorLib.Pages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static System.Net.WebRequestMethods;

namespace BetterCodeBox.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> ExitCommand { get; }
    public ReactiveCommand<Unit, Unit> ExportCommand { get; }
    public ReactiveCommand<Unit, Unit> ImportCommand { get; }
    public Interaction<Unit, Unit> ExitInteraction { get; }

    public MainWindowViewModel()
    {
        if(!Avalonia.Controls.Design.IsDesignMode)
        {
        }
        ExitCommand = ReactiveCommand.CreateFromTask(OnExit);
        ExportCommand = ReactiveCommand.CreateFromTask(OnExport);
        ImportCommand = ReactiveCommand.CreateFromTask(OnImport);
        ExitInteraction = new Interaction<Unit, Unit>();
    }

    private async Task OnExit()
    {
        await ExitInteraction.Handle(Unit.Default);
    }

    private async Task OnExport()
    {
        var filePicker = new AvaloniaFilePickerService();
        var filePath = await filePicker.SaveFile();
        if(filePath is not null)
        {
            // Write to file
            using var writer = new StreamWriter(filePath);
            string json = JSONHelper.SerializeJson(Home.Analyzers);
            await writer.WriteLineAsync(json);
        }
    }
    
    private async Task OnImport()
    {
        var filePicker = new AvaloniaFilePickerService();
        var filePath = await filePicker.SelectFile();
        if(filePath is not null)
        {
            // Read from file
            using var reader = new StreamReader(filePath);
            string json = await reader.ReadToEndAsync();
            var data = JSONHelper.ParseJson(json);
            Home.Analyzers.Clear();
            foreach(var analyzer in data.Analyzers)
            {
                Home.Analyzers.Add(new FakeCodeAnalyzer(analyzer.Title, analyzer.FileTitle, analyzer.Results));
            }
            Home.UpdateResults();
        }
    }
}

internal class FakeCodeAnalyzer : ICodeAnalyzer
{
    private readonly string _analyzerTitle;
    private readonly string _analyzerFileTitle;
    private readonly List<Result> _analyzerResults;

    public FakeCodeAnalyzer(string analyzerTitle, string analyzerFileTitle, List<Result> analyzerResults)
    {
        _analyzerTitle = analyzerTitle;
        _analyzerFileTitle = analyzerFileTitle;
        _analyzerResults = analyzerResults;
    }
    
    public string GetTitle() => _analyzerTitle;
    public string GetFileTitle() => _analyzerFileTitle;
    public Task Analyze(ProjectData project, IConfiguration configuration, ILogger logger)
    {
        return Task.CompletedTask;
    }

    public List<Result> GetResultAsJson() => _analyzerResults;
}
