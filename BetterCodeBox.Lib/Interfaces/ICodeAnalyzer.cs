using BetterCodeBox.Lib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BetterCodeBox.Lib.Interfaces;

public interface ICodeAnalyzer
{
    public string GetTitle();
    public string GetFileTitle();
    public Task Analyze(ProjectData project, IConfiguration configuration, ILogger logger);
    public List<Result> GetResultAsJson();
}