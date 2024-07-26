// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using BetterCodeBox;
using BetterCodeBox.Analyzers;
using BetterCodeBox.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

string solutionPath = args.Length > 0 ? args[0] : throw new ArgumentException("Path to the solution is required");
var configurationBuilder = new ConfigurationBuilder();
if (File.Exists("appsettings.json"))
{
    configurationBuilder.AddJsonFile("appsettings.json");
}

var configuration = configurationBuilder.Build();
ILogger logger = new BIABLogger();
ProjectData project = new ProjectData(solutionPath, logger);

foreach (var analyzer in analyzers)
{
    logger.LogInformation($"Analyzing with {analyzer.GetTitle()}");
    await analyzer.Analyze(project, configuration, logger);
}

string resultDirectory = configuration["ResultDirectory"] ?? "results";

if (!Directory.Exists(resultDirectory))
{
    Directory.CreateDirectory(resultDirectory);
}

foreach (var analyzer in analyzers)
{
    logger.LogInformation($"Writing results for {analyzer.GetTitle()}");
    List<Result> results = analyzer.GetResultAsJson();
    string title = analyzer.GetFileTitle();
    string resultPath = Path.Combine(resultDirectory, title + ".json");
    string result = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(resultPath, result);
    logger.LogInformation($"Results written to {resultPath}");
}