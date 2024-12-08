using System.Globalization;

namespace Fagkaffe.Helpers;

public static class EverythingHelper
{
    public static string DoubleToString(double n)
        => n.ToString("0.0000", CultureInfo.InvariantCulture);

    public static DateTime ConvertToCEST(DateTime value)
    {
        var timezone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        return TimeZoneInfo.ConvertTimeFromUtc(value, timezone);
    }

    public static string EscapeChars(this string input)
        => input.Replace(".", @"\.");

    public static string GetChatlogFilename()
        => $"./out/chatlogs/chat_{DateTime.Now:yyyy-MM-ddTHH:mm:ssZ}.txt";
}