#region [ Using ]

using Fagkaffe;
using Fagkaffe.CommandLine;
using Fagkaffe.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ClientModel;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

#endregion

#region [ Step 1: Import packages ]

using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;

#endregion

#region [ Configuration ]

ConsoleStateOptions options = new(10);

var hostBuilder = Host.CreateApplicationBuilder(args);
hostBuilder.Configuration.AddUserSecrets<Program>();

var loglevelOption = ArgumentHelpers.GetLogLevelOption();
var maxChatMessagesOption = ArgumentHelpers.GetMaxMessagesOption();

Action<LogLevel, int> handle = (logLevel, maxChatMessages) =>
{
    hostBuilder.Services.RegisterLogging(logLevel);
    options = new(maxChatMessages);
};
var rootCommand = ArgumentHelpers.GetRootCommand(
    loglevelOption,
    maxChatMessagesOption
);
rootCommand.SetHandler(
    handle,
    loglevelOption,
    maxChatMessagesOption
);
await new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .Build()
    .InvokeAsync(args);

#endregion

#region [ Initialization ]

Uri azureOpenAIEndpoint = new(hostBuilder.Configuration["AZURE_OPENAI_API_ENDPOINT"]!);
ApiKeyCredential azureOpenAIApiKey = new(hostBuilder.Configuration["AZURE_OPENAI_API_KEY"]!);
string azureOpenAIModel = hostBuilder.Configuration["AZURE_OPENAI_MODEL"]!;

Uri ollamaEndpoint = new(hostBuilder.Configuration["OLLAMA_API_ENDPOINT"]!);
string ollamaModel = "llama3.2";

hostBuilder.Services.RegisterDependencies();

var state = new ConsoleState(options);

Action<object?, ConsoleCancelEventArgs> consoleCancelHandler = async (sender, eventArgs) =>
{
    var filename = $"out/chatlogs/chat_{DateTime.Now}.txt";
    if (state.ChatMessages.Count > 1)
    {
        await FileHelper.WriteFileAsync(
            filename,
            state.PrettifyHistory()
        );

        ConsoleHelper.WriteLine(
            $"\n\nHistorikk lagret -> {filename}",
            ConsoleUser.System
        );
    }
};

ConsoleHelper.Init(consoleCancelHandler);

#endregion

#region [ Step 2: Create a chat client ]

// Create the underlying OpenAI client
AzureOpenAIClient openAIClient = new(azureOpenAIEndpoint, azureOpenAIApiKey);

// ... or any other client
OllamaChatClient ollamaChatClient = new(ollamaEndpoint, ollamaModel);

#endregion

#region [ Step 3: Register client and configuration ]

hostBuilder.Services.AddChatClient(builder => builder
        // Where the magic happens
        .UseFunctionInvocation()
        .Use(openAIClient.AsChatClient(azureOpenAIModel)));

#endregion

var app = hostBuilder.Build();

#region [ Step 4: Register tools ]

IList<Delegate> tools = app.GetTools();
ChatOptions chatOptions = new()
{
    // Add tools to chat message
    Tools = [
        .. tools
        .Select(x => AIFunctionFactory.Create(x))
    ],
    // Optional: Set tool mode. Auto, Required or None
    ToolMode = ChatToolMode.Auto,

    // other config ...
    Temperature = 0.3f,
};

#endregion

#region [ Step 5: Use in application ]

async Task Chat(string input)
{
    // Get service from IServiceProvider
    var client = app.Services.GetService<IChatClient>()!;

    // Append chat message
    state.AppendHistory(new ChatMessage(ChatRole.User, input));

    // Send message to AI model, using chat messages and chat options configured with tools
    var result = await client.CompleteAsync(
        state.ChatMessages,
        chatOptions
    );

    // State handling, writing to stdout etc.
    state.AddChatCompletion(result);

    foreach (var message in result.Choices)
    {
        state.AppendHistory(message);
        ConsoleHelper.WriteLine(message.Text, ConsoleUser.Assistant);
    }
}

#endregion

#region [ Program ]

while (true)
{
    string? input = ConsoleHelper.ReadLine();
    switch (input)
    {
        case null or "":
            break;
        case "clear()":
            Clear();
            break;
        case "print()":
            Print();
            break;
        default:
            await Chat(input);
            break;
    }
}

#endregion

#region [ Commands ]

void Print()
{
    ConsoleHelper.WriteLine(
        state.PrettifyHistory(),
        ConsoleUser.System
    );
}

void Clear()
{
    state.ClearHistory();
    ConsoleHelper.WriteLine(
        "Chathistorikk slettet!\n",
        ConsoleUser.SystemInformation
    );
}

#endregion
