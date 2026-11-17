namespace MeDotNet.Models;

/// <summary>Whole-minute reading estimate for a Markdown body, never less than one.</summary>
public static class ReadingTime
{
    private const int WordsPerMinute = 220;

    public static int Minutes(string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return 1;
        var words = body.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
        return Math.Max(1, (int)Math.Ceiling(words / (double)WordsPerMinute));
    }
}
