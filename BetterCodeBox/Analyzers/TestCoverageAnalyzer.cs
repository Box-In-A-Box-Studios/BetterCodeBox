using BetterCodeBox.Interfaces;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BetterCodeBox.Analyzers;

public class TestCoverageAnalyzer : ICodeAnalyzer
{
    private List<(string,bool)> _results = new(); 
    public string GetTitle() => "Methods Not Covered by Tests";

    public string GetFileTitle() => "methods-not-covered-by-tests";

    public Task Analyze(ProjectData project, IConfiguration configuration, ILogger logger)
    {
        // Get the methods from the files
        var methods = new List<string>();
        foreach (var file in project.NonTestFiles)
        {
            var text = File.ReadAllText(file);
            var tree = CSharpSyntaxTree.ParseText(text);
            var root = tree.GetRoot();

            var methodDeclarations = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
            foreach (var method in methodDeclarations)
            {
                methods.Add(IdentifierHelper.GetMethodIdentifier(method));
            }
        }
        
        // Get all of the methods that are called in the test files
        var calledMethods = new List<string>();
        foreach (var testFile in project.TestFiles)
        {
            var text = File.ReadAllText(testFile);
            var tree = CSharpSyntaxTree.ParseText(text);
            var root = tree.GetRoot();

            var methodInvocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>();
            foreach (var method in methodInvocations)
            {
                MethodDeclarationSyntax? methodDeclaration = null;
                if (method.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    methodDeclaration = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                        .FirstOrDefault(x => x.Identifier.ToString() == memberAccess.Name.ToString());
                }
                else if (method.Expression is IdentifierNameSyntax identifier)
                {
                    methodDeclaration = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                        .FirstOrDefault(x => x.Identifier.ToString() == identifier.Identifier.ToString());
                }
                if (methodDeclaration != null)
                {
                    calledMethods.Add(IdentifierHelper.GetMethodIdentifier(methodDeclaration));
                }
            }
        }
        
        // remove the methods that are called from the methods list
        var results = new List<(string,bool)>();
        foreach (var method in calledMethods)
        {
            methods.Remove(method);
            results.Add((method, true));
        }
        results.AddRange(methods.Select(x => (x, false)));
        
        _results = results;
        return Task.CompletedTask;
    }

    public List<Result> GetResultAsJson()
    {
        return _results.Select(r =>
            new Result()
            {
                Identifier = r.Item1,
                Type = r.Item2 ? ResultType.Success : ResultType.Error,
                Value = r.Item2 ? "Covered" : "Not Covered"
            }).ToList();
    }
}