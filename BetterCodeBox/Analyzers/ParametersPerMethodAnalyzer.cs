using BetterCodeBox.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BetterCodeBox.Analyzers;

public class ParametersPerMethodAnalyzer : ICodeAnalyzer
{
    private int _maxParameterCount = 4;
    private List<(string, int)> _results = new();
    public string GetTitle() => $"Methods With More than {_maxParameterCount} Parameters";

    public string GetFileTitle() => $"methods-with-more-than-{_maxParameterCount}-parameters";

    public Task Analyze(ProjectData project, IConfiguration configuration, ILogger logger)
    {
        _maxParameterCount = configuration["MaxParameters"] != null ? int.Parse(configuration["MaxParameters"]!) : _maxParameterCount;
        foreach (var file in project.NonTestFiles)
        {
            var methods = GetParametersPerMethod(file);
            foreach (var method in methods)
            {
                _results.Add((method.Key, method.Value));
            }
        }
        
        return Task.CompletedTask;
    }
    
    private static Dictionary<string, int> GetParametersPerMethod(string filePath)
    {
        var text = File.ReadAllText(filePath);
        var tree = CSharpSyntaxTree.ParseText(text);
        var root = tree.GetRoot();

        var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

        var result = new Dictionary<string, int>();

        foreach (var method in methods)
        {
            string identifier = IdentifierHelper.GetMethodIdentifier(method);
            int parameterCount = 0;
            // Add Parameters to the identifier
            if (method.ParameterList != null)
            {
                parameterCount = method.ParameterList.Parameters.Count;
            }
            
            if (result.ContainsKey(identifier))
            {
                if (method.Modifiers.Any(SyntaxKind.OverrideKeyword))
                {
                    result[identifier] = parameterCount;
                }
            }
            else
            {
                result.Add(identifier, parameterCount);
            }
        }

        return result;
    }

    public List<Result> GetResultAsJson()
    {
        // Convert the results to JSON
        List<Result> results = _results.Select(r =>
            new Result()
            {
                Identifier = r.Item1,
                Type = r.Item2 < _maxParameterCount ? ResultType.Success : ResultType.Error,
                Value = r.Item2.ToString()
            }).ToList();
        return results;
    }
}