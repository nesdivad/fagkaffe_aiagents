using System.Text;
using Fagkaffe.Helpers;
using Microsoft.Extensions.AI;

namespace Fagkaffe.CommandLine;

public class ConsoleState(ConsoleStateOptions options)
{
    private readonly ChatMessage SystemMessage = new(ChatRole.System, "Du er en chatbot som svarer på alle spørsmål på norsk.");
    public IList<ChatMessage> ChatMessages { get; private set; } = [];
    public ConsoleStateOptions Options { get; } = options;
    public Statistics Statistics { get; } = new();

    #region [ History ]

    public void AddChatCompletion(ChatCompletion result)
    {
        Statistics.AddInputTokens(result?.Usage?.InputTokenCount);
        Statistics.AddOutputTokens(result?.Usage?.OutputTokenCount);
    }

    public void AppendHistory(ChatMessage chatMessage)
    {
        if (ChatMessages.Count is 0)
        {
            ChatMessages.Add(SystemMessage);
        }
        if (ChatMessages.Count == Options.MaxChatMessages)
        {
            ChatMessages.RemoveAt(0);
        }

        ChatMessages.Add(chatMessage);
    }

    public void ClearHistory()
    {
        ChatMessages = [];
    }

    public string PrettifyHistory()
    {
        var sb = new StringBuilder();

        sb.AppendLine("\nChathistorikk");
        sb.AppendLine(string.Concat(Enumerable.Repeat("=", 100)));

        for (var i = 0; i < ChatMessages.Count; i++)
        {
            var message = ChatMessages[i];
            var text = string.IsNullOrEmpty(message.Text) ? "<empty string>" : message.Text;
            sb.AppendLine($"§{message.Role.Value}: {text}");
            if (message.RawRepresentation is OpenAI.Chat.ChatCompletion chatCompletion)
            {
                foreach (var tool in chatCompletion.ToolCalls)
                {
                    sb.AppendLine($">> function name: \x1b[7m{tool.FunctionName}\x1b[27m | function args: \x1b[1m{tool.FunctionArguments.ToString()}\x1b[22m");
                }
            }
            else if (message.RawRepresentation is OpenAI.Chat.ToolChatMessage toolChatMessage)
            {
                sb.AppendLine($">> {toolChatMessage.Content}");
            }

            if (i + 1 < ChatMessages.Count)
            {
                sb.AppendLine(string.Concat(Enumerable.Repeat("- ", 50)));
            }
        }

        sb.AppendLine(string.Concat(Enumerable.Repeat("=", 100)));

        sb.Append(Statistics.ToString());

        return sb.ToString();
    }

    public async Task<bool> SaveChatlog()
    {
        if (ChatMessages.Count <= 1) return false;
        var filename = EverythingHelper.GetChatlogFilename();
        await FileHelper.WriteFileAsync(filename, PrettifyHistory());

        return true;
    }

    #endregion
}

public class ConsoleStateOptions(int maxChatMessages)
{
    public int MaxChatMessages { get; } = maxChatMessages;
}