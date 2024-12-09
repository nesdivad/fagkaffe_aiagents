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
hostBuilder.Services.RegisterDependencies();

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
Uri azureOpenAIEndpointNorway = new(hostBuilder.Configuration["AZURE_OPENAI_API_ENDPOINT_NORWAY"]!);
ApiKeyCredential azureOpenAIApiKey = new(hostBuilder.Configuration["AZURE_OPENAI_API_KEY"]!);
ApiKeyCredential azureOpenAIApiKeyNorway = new(hostBuilder.Configuration["AZURE_OPENAI_API_KEY_NORWAY"]!);
string azureOpenAIModel = hostBuilder.Configuration["AZURE_OPENAI_MODEL"]!;

Uri ollamaEndpoint = new(hostBuilder.Configuration["OLLAMA_API_ENDPOINT"]!);
string ollamaModel = "llama3.2";

var state = new ConsoleState(options);

Action<object?, ConsoleCancelEventArgs> consoleCancelHandler = async (sender, eventArgs) =>
{
    await state.SaveChatlog();

    ConsoleHelper.EraseLine();
    ConsoleHelper.WriteLine("👋");
    Console.CursorVisible = true; 
};

Dictionary<string, Action> commands = new()
{
    { "print()", Print },
    { "clear()", Clear },
    { "help()", Help }
};

ConsoleHelper.Init(consoleCancelHandler, commands);

#endregion

#region [ Step 2: Create a chat client ]

// Create the underlying OpenAI client
AzureOpenAIClient openAIClient = new(azureOpenAIEndpoint, azureOpenAIApiKey);

// ... or any other client
OllamaChatClient ollamaChatClient = new(ollamaEndpoint, ollamaModel);

#endregion

#region [ Step 3: Register client and configuration ]

IChatClient chatClient = openAIClient
    .AsChatClient(azureOpenAIModel)
    .AsBuilder()
    // where the magic happens
    .UseFunctionInvocation()
    .Build();

hostBuilder.Services.AddChatClient(chatClient);

#endregion

#region [ Build app ]

var app = hostBuilder.Build();

#endregion

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
    #region stuff

    CancellationTokenSource cancellationTokenSource = new();
    CancellationToken token = cancellationTokenSource.Token;
    ConsoleHelper.SetCursorVisible(false);
    ConsoleHelper.Wait(token);

    #endregion
    
    // Get service from IServiceProvider
    IChatClient client = app.Services.GetService<IChatClient>()!;

    // Append chat message
    state.AppendHistory(new ChatMessage(ChatRole.User, input));

    // Send message to AI model, using chat messages and chat options configured with tools
    var result = await client.CompleteAsync(
        state.ChatMessages,
        chatOptions,
        token
    );

    #region morestuff

    cancellationTokenSource.Cancel();
    ConsoleHelper.EraseLine();
    ConsoleHelper.SetCursorVisible(true);

    // State handling, writing to stdout etc.
    state.AddChatCompletion(result);

    foreach (var message in result.Choices)
    {
        state.AppendHistory(message);
        ConsoleHelper.WriteLine(message.Text, ConsoleUser.Assistant);
    }

    #endregion
}

#endregion

#region [ Program ]

while (true)
{
    string? input = ConsoleHelper.ReadLine();
    if (string.IsNullOrEmpty(input)) continue;
    if (commands.TryGetValue(input, out Action? action)) 
        action();
    else
        await Chat(input);
}

#endregion

#region [ Commands ]

void Print()
{
    ConsoleHelper.WriteLine(state.PrettifyHistory());
}

void Clear()
{
    Task.Run(state.SaveChatlog);
    state.ClearHistory();
    ConsoleHelper.Clear();
}

void Help()
{
    ConsoleHelper.WriteLine("Examples:\n");
    ConsoleHelper.WriteLine("Read <filename> and review the contents. Can you recommend some improvements?");
    ConsoleHelper.WriteLine("I want to travel from Byparken to Solheimsviken. Can you find the next scheduled departure?");

    ConsoleHelper.WriteLine("\nOptions:");
    foreach (var option in rootCommand.Options)
    {
        var spacing = string.Concat(
            Enumerable.Repeat(" ", 40 - option.Name.Length)
        );
        
        ConsoleHelper.Write(
            $"--{option.Name}{spacing}{option.Description}\n"
        );
    }
}


#endregion
