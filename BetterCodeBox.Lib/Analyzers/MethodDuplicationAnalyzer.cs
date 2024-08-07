using System.Text.RegularExpressions;
using BetterCodeBox.Lib.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BetterCodeBox.Lib.Analyzers;

public class MethodDuplicationAnalyzer : ICodeAnalyzer
{
    private List<(string,string,double)> _results = new();
    private int methodCount = 0;
    public string GetTitle() => "Methods that are duplicates";

    public string GetFileTitle() => "methods-that-are-duplicates";

    public Task Analyze(ProjectData project, IConfiguration configuration, ILogger logger)
    {
        var methods = new List<MethodDeclarationSyntax>();
        foreach (var file in project.NonTestFiles)
        {
            var text = File.ReadAllText(file);
            var tree = CSharpSyntaxTree.ParseText(text);
            var root = tree.GetRoot();

            var methodDeclarations = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
            // Remove Methods With 3 or less lines
            methodDeclarations = methodDeclarations.Where(x => x.GetLocation().GetLineSpan().EndLinePosition.Line - x.GetLocation().GetLineSpan().StartLinePosition.Line > 3).ToList();
            methods.AddRange(methodDeclarations);
        }
        methodCount = methods.Count;
        
        var duplicateMethods = new List<(string,string,double)>();
        // Compare each method to every other method
        for (int i = 0; i < methods.Count; i++)
        {
            for (int j = i + 1; j < methods.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }
                var method1 = methods[i];
                var method2 = methods[j];
                // Get the text of the methods
                var method1Text = method1.NormalizeWhitespace().ToFullString();
                var method2Text = method2.NormalizeWhitespace().ToFullString();
                
                
                // Compare the text of the methods
                var x = CompareMethodText(method1Text, method2Text);
                if (x > 0.8)
                {
                    duplicateMethods.Add(new (IdentifierHelper.GetMethodIdentifier(method1),IdentifierHelper.GetMethodIdentifier(method2), x));
                }
            }
        }
        
        _results = duplicateMethods;
        
        return Task.CompletedTask;
    }
    
    
    private static double CompareMethodText(string method1, string method2)
    {
        // Remove Comments
        method1 = Regex.Replace(method1, @"\/\/.*", "");
        method2 = Regex.Replace(method2, @"\/\/.*", "");
        method1 = Regex.Replace(method1, @"\/\*.*\*\/", "");
        method2 = Regex.Replace(method2, @"\/\*.*\*\/", "");
        
        // Remove Whitespace
        method1 = Regex.Replace(method1, @"\s+", "");
        method2 = Regex.Replace(method2, @"\s+", "");
        
        // Remove String Literals
        method1 = Regex.Replace(method1, "\".*\"", "");
        method2 = Regex.Replace(method2, "\".*\"", "");
        
        // Get the percentage of the same code in the methods by comparing line by line
        int sameCount = 0;
        int totalLines = 0;
        var method1Lines = method1.Split('\n');
        var method2Lines = method2.Split('\n');
        // double for loop to compare each line of the methods
        for (int i = 0; i < method1Lines.Length; i++)
        {
            for (int j = 0; j < method2Lines.Length; j++)
            {
                if (method1Lines[i] == method2Lines[j])
                {
                    sameCount++;
                    break;
                }
            }
            totalLines++;
        }
        
        return (double)sameCount / totalLines;
    }

    public List<Result> GetResultAsJson()
    {
        return _results.OrderByDescending(x=>x.Item3).Select(r =>
            new Result()
            {
                Identifier = r.Item1,
                Type = ResultType.Error,
                Value = r.Item3.ToString(),
                SecondaryIdentifier = r.Item2
            }).ToList();
    }
    
    public int GetScore()
    {
        return 100 - ( _results.Count * 100 / methodCount);
    }
}