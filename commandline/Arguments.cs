using System.CommandLine;
using Microsoft.Extensions.Logging;

namespace Fagkaffe.CommandLine;

public static class ArgumentHelpers
{
    public static RootCommand GetRootCommand(params Option[] options)
    {
        RootCommand rootCommand = [];
        foreach (var option in options)
            rootCommand.Add(option);

        return rootCommand;
    }

    public static Option<LogLevel> GetLogLevelOption()
    {
        Option<LogLevel> option = new(
            name: "--loglevel",
            description: "Set logging level",
            getDefaultValue: () => LogLevel.Error
        );
        option.AddAlias("-l");
        option.AddCompletions(
            "Debug",
            "Information",
            "Warning",
            "Error"
        );

        return option;
    }

    public static Option<int> GetMaxMessagesOption()
    {
        Option<int> option = new(
            name: "--history",
            description: "set max number of chat messages",
            getDefaultValue: () => 10
        );
        option.AddAlias("-h");

        return option;
    }
}