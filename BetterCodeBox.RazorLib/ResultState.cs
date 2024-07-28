using BetterCodeBox.Lib;

namespace BetterCodeBox.RazorLib;

public static class ResultState
{
    public static readonly List<AnalysisData> Results = new();
    public static event EventHandler? OnChanged;
    
    
    public static void SetResult(List<AnalysisData> data)
    {
        Results.Clear();
        Results.AddRange(data);
        OnChanged?.Invoke(null, EventArgs.Empty);
    }
    
    public static void AddResult(AnalysisData data)
    {
        Results.Add(data);
        OnChanged?.Invoke(null, EventArgs.Empty);
    }

    public static void Clear()
    {
        Results.Clear();
        OnChanged?.Invoke(null, EventArgs.Empty);
    }
    
    public static bool IsEmpty()
    {
        return Results.Count == 0;
    }
}