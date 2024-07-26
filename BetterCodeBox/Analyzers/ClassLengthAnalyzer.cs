using System.Text.Json;
using BetterCodeBox.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BetterCodeBox.Analyzers;

public class ClassLengthAnalyzer : ICodeAnalyzer
{
    public string GetTitle() => $"Files over {_maxLines} lines";
    public string GetFileTitle() => $"files-over-{_maxLines}-lines";

    private List<(string,int)> _results = new();
    private int _maxLines = 250;
    public Task Analyze(ProjectData project, IConfiguration configuration, ILogger logger)
    {
        _maxLines = configuration["MaxLines"] != null ? int.Parse(configuration["MaxLines"]!) : _maxLines;
        // Foreach file in the project, get the line count of each class
        foreach (var file in project.NonTestFiles)
        {
            var lineCount = GetLineCount(file);
            _results.Add((file, lineCount));
        }
        return Task.CompletedTask;
    }

    public List<Result> GetResultAsJson()
    {
        // Convert the results to JSON
        List<Result> results = _results.Select(r =>
            new Result()
            {
                Identifier = r.Item1,
                Type = r.Item2 < _maxLines ? ResultType.Success : ResultType.Error,
                Value = r.Item2.ToString()
            }).ToList();
        return results;
    }
    
    private static int GetLineCount(string filePath)
    {
        return File.ReadAllLines(filePath).Length;
    }
}