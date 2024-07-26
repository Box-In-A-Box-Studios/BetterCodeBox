namespace BetterCodeBox;

public class Result
{
    public ResultType Type { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public enum ResultType
{
    Success,
    Warning,
    Error
}