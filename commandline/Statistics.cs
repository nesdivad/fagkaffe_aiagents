using System.Text;

namespace Fagkaffe.CommandLine;

public class Statistics
{
    public int InputTokens { get; private set; } = 0;
    public int OutputTokens { get; private set; } = 0;

    public void AddInputTokens(int? tokens)
    {
        InputTokens += tokens ?? 0;
    }

    public void AddOutputTokens(int? tokens)
    {
        OutputTokens += tokens ?? 0;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine(string.Concat(Enumerable.Repeat(" ", 100)));
        sb.AppendLine($"Input tokens: {InputTokens}");
        sb.AppendLine($"Output tokens: {OutputTokens}");

        return sb.ToString();
    }
}
