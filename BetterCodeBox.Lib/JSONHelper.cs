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
    public string Title { get; set; }
    public string FileTitle { get; set; }
    
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
        JsonData jsonData = new JsonData();
        jsonData.Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        jsonData.Version = "1.0";
        jsonData.Analyzers = new List<AnalysisData>();
        
        foreach (var analyzer in data)
        {
            AnalysisData analysisData = new AnalysisData();
            analysisData.Title = analyzer.GetTitle();
            analysisData.FileTitle = analyzer.GetFileTitle();
            analysisData.Results = analyzer.GetResultAsJson();
            jsonData.Analyzers.Add(analysisData);
        }
        
        return JsonSerializer.Serialize(jsonData);
    }
}