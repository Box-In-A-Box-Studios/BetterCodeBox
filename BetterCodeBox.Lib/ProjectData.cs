using Microsoft.Extensions.Logging;

namespace BetterCodeBox.Lib;

public class ProjectData
{
    public List<string> NonTestFiles { get; init; } = new();
    public List<string> TestFiles { get; init; } = new();
    public List<string> TestProjects { get; init; } = new();
    public List<string> NonTestProjects { get; init; } = new();


    public ProjectData(string path, ILogger logger)
    {
        logger.LogInformation($"Analyzing {path}");
        string[] csprojFiles = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories);
        foreach (var file in csprojFiles)
        {
            string text = File.ReadAllText(file);
            string? directory = Path.GetDirectoryName(file);
            if (directory == null)
                continue;
            if (text.Contains("Microsoft.NET.Test.Sdk"))
            {
                TestProjects.Add(directory);
            }else
            if (text.Contains("NUnit"))
            {
                TestProjects.Add(directory);
            }else
            if (text.Contains("xunit"))
            {
                TestProjects.Add(directory);
            }
            else
            {
                NonTestProjects.Add(directory);
            }
        }
        logger.LogInformation($"Found {TestProjects.Count} test projects");

        string[] ignorePaths = new string[] { "bin\\", "obj\\", "Properties\\" };
        string[] ignoreFiles = new string[] { "AssemblyInfo.cs", "Usings.cs" };
        
        foreach (var testProject in TestProjects)
        {
            string[] files = Directory.GetFiles(testProject, "*.cs", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (ignorePaths.Any(x => file.Contains(x)))
                    continue;
                if (ignoreFiles.Any(x => file.EndsWith(x)))
                    continue;
                TestFiles.Add(file);
            }
        }
        foreach (var nonTestProject in NonTestProjects)
        {
            string[] files = Directory.GetFiles(nonTestProject, "*.cs", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (ignorePaths.Any(x => file.Contains(x)))
                    continue;
                if (ignoreFiles.Any(x => file.EndsWith(x)))
                    continue;
                NonTestFiles.Add(file);
            }
        }
        
        logger.LogInformation($"Found {NonTestFiles.Count} non-test files");
        logger.LogInformation($"Found {TestFiles.Count} test files");
    }
}