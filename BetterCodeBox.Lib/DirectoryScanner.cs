using BetterCodeBox.Lib.Analyzers;
using BetterCodeBox.Lib.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BetterCodeBox.Lib;

public class DirectoryScanner
{
    public static async Task<List<ICodeAnalyzer>> ScanDirectory(string directory, ILogger logger, IConfiguration configuration)
    {
        List<ICodeAnalyzer> analyzers = new List<ICodeAnalyzer>
        {
            new ClassLengthAnalyzer(),
            new MethodLengthAnalyzer(),
            new ParametersPerMethodAnalyzer(),
            new MethodMaxDepthAnalyzer(),
            new MethodMaxBlockAnalyzer(),
            new TestCoverageAnalyzer(),
            new MethodDuplicationAnalyzer()
        };
        
        ProjectData project = new ProjectData(directory, logger);

        foreach (var analyzer in analyzers)
        {
            logger.LogInformation($"Analyzing with {analyzer.GetTitle()}");
            await analyzer.Analyze(project, configuration, logger);
        }

        return analyzers;
    }
}