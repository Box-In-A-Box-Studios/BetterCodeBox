namespace BetterCodeBox.Lib;

public class Result
{
    public ResultType Type { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? SecondaryIdentifier { get; set; }
}

public enum ResultType
{
    Success,
    Warning,
    Error
}