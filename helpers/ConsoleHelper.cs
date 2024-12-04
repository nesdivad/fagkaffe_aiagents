namespace Fagkaffe.Helpers;

public static class ConsoleHelper
{
    public static void Init(
        Action<object?, ConsoleCancelEventArgs> ConsoleCancelHandler)
    {
        Console.Clear();
        Console.CancelKeyPress += new ConsoleCancelEventHandler(
            ConsoleCancelHandler
        );
    }

    public static void WriteLine(string? text, ConsoleUser user)
    {
        SetForegroundColour(user);
        Console.WriteLine(text);
    }

    public static string? ReadLine()
    {
        SetForegroundColour(ConsoleUser.User);
        Console.Write("\n>> ");
        return Console.ReadLine();
    }

    private static void SetForegroundColour(ConsoleUser user)
    {
        Console.ForegroundColor = user switch
        {
            ConsoleUser.User => ConsoleColor.White,
            ConsoleUser.Assistant => ConsoleColor.Green,
            ConsoleUser.System => ConsoleColor.Cyan,
            ConsoleUser.SystemInformation => ConsoleColor.DarkRed,
            _ => ConsoleColor.White
        };
    }
}

public enum ConsoleUser
{
    User,
    Assistant,
    System,
    SystemInformation
}