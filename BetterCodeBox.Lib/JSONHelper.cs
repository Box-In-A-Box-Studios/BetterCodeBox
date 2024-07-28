using System.Text.Json;
using BetterCodeBox.Lib.Interfaces;

namespace BetterCodeBox.Lib;

public class JsonData
{
    public string Date { get; set; }
    public string Version { get; set; }
    
    public List<AnalysisData> Analyzers { get; set; }
}

public class AnalysisData
{
    public AnalysisData(ICodeAnalyzer analyzer)
    {
        Title = analyzer.GetTitle();
        FileTitle = analyzer.GetFileTitle();
        Results = analyzer.GetResultAsJson();
        CurrentScore = analyzer.GetScore();
    }
    
    public AnalysisData()
    {
    }

    public string Title { get; set; }
    public string FileTitle { get; set; }
    public int CurrentScore { get; set; }
    
    public List<Result> Results { get; set; }
}

public class JSONHelper
{
    public static JsonData ParseJson(string json)
    {
        return JsonSerializer.Deserialize<JsonData>(json);
    }
    
    public static string SerializeJson(List<ICodeAnalyzer> data)
    {
        List<AnalysisData> analysisData = new();
        foreach (var analyzer in data)
        {
            analysisData.Add(new AnalysisData(analyzer));
        }
        return SerializeJson(analysisData);
    }
    
    public static string SerializeJson(List<AnalysisData> data)
    {
        JsonData jsonData = new JsonData();
        jsonData.Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        jsonData.Version = "1.0";
        jsonData.Analyzers = data;
        
        return JsonSerializer.Serialize(jsonData);
    }
}