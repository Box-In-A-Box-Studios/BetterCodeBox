using BetterCodeBox.Lib;
using BetterCodeBox.Lib.Interfaces;

namespace BetterCodeBox.RazorLib;

public static class FileActions
{
    public static async Task SaveFile(string filePath, List<ICodeAnalyzer> analyzers)
    {
        if(filePath is not null)
        {
            // Write to file
            using var writer = new StreamWriter(filePath);
            string json = JSONHelper.SerializeJson(analyzers);
            await writer.WriteLineAsync(json);
        }
    }

    public static async Task SaveFile(string filePath, List<AnalysisData> analyzers)
    {
        if(filePath is not null)
        {
            // Write to file
            using var writer = new StreamWriter(filePath);
            string json = JSONHelper.SerializeJson(analyzers);
            await writer.WriteLineAsync(json);
        }
    }

    public static async Task<List<AnalysisData>> LoadFile(string filePath)
    {
        if(filePath is not null)
        {
            // Read from file
            using var reader = new StreamReader(filePath);
            string json = await reader.ReadToEndAsync();
            return JSONHelper.ParseJson(json).Analyzers;
        }
        return new List<AnalysisData>();
    }
}