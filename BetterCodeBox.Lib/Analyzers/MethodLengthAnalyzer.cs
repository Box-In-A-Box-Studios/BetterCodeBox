using BetterCodeBox.Lib.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BetterCodeBox.Lib.Analyzers;

public class MethodLengthAnalyzer : ICodeAnalyzer
{
    private int _maxLines = 100;
    private List<(string, int)> _results = new();
    
    public string GetTitle() => $"Methods over {_maxLines} lines";

    public string GetFileTitle() => $"methods-over-{_maxLines}-lines";
    
    public Task Analyze(ProjectData project, IConfiguration configuration, ILogger logger)
    {
        _maxLines = configuration["MaxLines"] != null ? int.Parse(configuration["MaxLines"]!) : _maxLines;
        foreach (var file in project.NonTestFiles)
        {
            var methods = GetMethodsAndLineCounts(file);
            foreach (var method in methods)
            {
                _results.Add((method.Key, method.Value));
            }
        }
        
        return Task.CompletedTask;
    }
    private static Dictionary<string, int> GetMethodsAndLineCounts(string filePath)
    {
        var text = File.ReadAllText(filePath);
        var tree = CSharpSyntaxTree.ParseText(text);
        var root = tree.GetRoot();

        var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

        var result = new Dictionary<string, int>();

        foreach (var method in methods)
        {
            var startLine = method.GetLocation().GetLineSpan().StartLinePosition.Line;
            var endLine = method.GetLocation().GetLineSpan().EndLinePosition.Line;
            var lineCount = endLine - startLine + 1;

            string identifier = IdentifierHelper.GetMethodIdentifier(method);
            
            // if it is already in the dictionary, check if its an overload, if it is, replace it
            if (result.ContainsKey(identifier))
            {
                if (method.Modifiers.Any(SyntaxKind.OverrideKeyword))
                {
                    result[identifier] = lineCount;
                }
            }
            else
            {
                result.Add(identifier, lineCount);
            }
        }

        return result;
    }

    public List<Result> GetResultAsJson()
    {
        List<Result> results = _results.OrderByDescending(x=>x.Item2).Select(r =>
            new Result()
            {
                Identifier = r.Item1,
                Type = (r.Item2 < _maxLines) ? ResultType.Success : ResultType.Error,
                Value = r.Item2.ToString()
            }).ToList();
        return results;
    }
}