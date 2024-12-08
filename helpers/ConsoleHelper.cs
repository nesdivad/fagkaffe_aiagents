namespace Fagkaffe.Helpers;

public static class ConsoleHelper
{

    private static readonly Dictionary<ConsoleUser, ConsoleColor> UserColors = new()
    {
        { ConsoleUser.User, ConsoleColor.White },
        { ConsoleUser.Assistant, ConsoleColor.Green },
        { ConsoleUser.System, ConsoleColor.Cyan },
        { ConsoleUser.SystemInformation, ConsoleColor.DarkRed }
    };
    
    public static void Init(
        Action<object?, ConsoleCancelEventArgs> consoleCancelHandler,
        Dictionary<string, Action> commands)
    {
        Console.Clear();
        Console.CancelKeyPress += new ConsoleCancelEventHandler(
            consoleCancelHandler
        );

        PrintWelcomeMessage(commands);
    }

    public static void PrintWelcomeMessage(Dictionary<string, Action> commands)
    {
        WriteLine("v0.0.1\n");
        WriteLine("The following helper commands are available:\n");
        WriteLine(string.Join(" | ", commands.Keys));
    }

    public static void Clear()
    {
        Console.Clear();
        WriteLine("Chat history deleted ...", ConsoleUser.SystemInformation);
    }

    public static void Write(char? character, ConsoleUser user = ConsoleUser.System)
        => Write(character.ToString(), user);

    public static void Write(string? text, ConsoleUser user = ConsoleUser.System)
    {
        SetForegroundColour(user);
        Console.Write(text);
        Console.ResetColor();
    }

    public static void WriteLine(string? text, ConsoleUser user = ConsoleUser.System)
    {
        SetForegroundColour(user);
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static string? ReadLine()
    {
        SetForegroundColour(ConsoleUser.User);
        Console.Write("\n>> ");
        return Console.ReadLine();
    }

    public static void Wait(CancellationToken token)
    {
        Task task = Task.Run(() => 
        {
            char[] chars = [ '|', '/', '-', '\\' ];
            int index = 0;

            while (true)
            {
                Thread.Sleep(200);
                token.ThrowIfCancellationRequested();
                MoveCursorLeft(1);
                Write(chars[index++ % chars.Length]);
            }
        }, token);
    }

    public static void EraseLine() => Console.Write("\x1b[2K\r");
    public static void MoveCursorLeft(int number) 
        => Console.Write($"\x1b[{number}D");
    public static void SetCursorVisible(bool visible)
        => Console.CursorVisible = visible;

    private static void SetForegroundColour(ConsoleUser user)
    {
        if (UserColors.TryGetValue(user, out var color))
        {
            Console.ForegroundColor = color;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}

public enum ConsoleUser
{
    User,
    Assistant,
    System,
    SystemInformation
}