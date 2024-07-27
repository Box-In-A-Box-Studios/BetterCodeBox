using BetterCodeBox.Lib.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BetterCodeBox.Lib.Analyzers;

public class MethodMaxBlockAnalyzer : ICodeAnalyzer
{
    private int _maxBlocks = 10;
    private List<(string, int)> _results = new();
    public string GetTitle() => $"Methods with more than {_maxBlocks} blocks";

    public string GetFileTitle() => $"methods-with-more-than-{_maxBlocks}-blocks";

    public Task Analyze(ProjectData project, IConfiguration configuration, ILogger logger)
    {
        _maxBlocks = configuration["MaxBlocks"] != null ? int.Parse(configuration["MaxBlocks"]!) : _maxBlocks;
        foreach (var file in project.NonTestFiles)
        {
            var methods = GetMaxNestPerMethod(file);
            foreach (var method in methods)
            {
                _results.Add((method.Key, method.Value.depth));
            }
        }
        
        return Task.CompletedTask;
    }
    
    // Method to return the maxmimum number of nests in a method
    private static Dictionary<string, (int count, int depth)> GetMaxNestPerMethod(string filePath)
    {
        var text = File.ReadAllText(filePath);
        var tree = CSharpSyntaxTree.ParseText(text);
        var root = tree.GetRoot();

        var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

        var result = new Dictionary<string, (int count, int depth)>();

        foreach (var method in methods)
        {
            string identifier = IdentifierHelper.GetMethodIdentifier(method);
            // Get the max nest
            var nests = method.DescendantNodes().OfType<BlockSyntax>().ToArray();
            int maxNest = nests.Length;
            
            if (result.ContainsKey(identifier))
            {
                if (method.Modifiers.Any(SyntaxKind.OverrideKeyword))
                {
                    result[identifier] = (nests.Length, maxNest);
                }
            }
            else
            {
                result.Add(identifier, (nests.Length, maxNest));
            }
        }

        return result;
    }


    public List<Result> GetResultAsJson()
    {
        var results = _results.OrderByDescending(x=>x.Item2).Select(r =>
            new Result()
            {
                Identifier = r.Item1,
                Type = (r.Item2 < _maxBlocks) ? ResultType.Success : ResultType.Error,
                Value = r.Item2.ToString()
            }).ToList();
        return results;
    }
    
    public int GetScore()
    {
        int overMax = _results.Count(x => x.Item2 > _maxBlocks);
        return 100 - (overMax * 100 / _results.Count);
    }
}