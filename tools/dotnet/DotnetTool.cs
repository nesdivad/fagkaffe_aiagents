using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Fagkaffe.Helpers;

namespace Fagkaffe.Tools.Dotnet;

[Description("Tools for running dotnet commands")]
public static class DotnetTool
{
    [Description("Run dotnet commands")]
    public static async Task<string?> BuildAsync(
        [Description("The dotnet command to run, e.g. 'build'")] string command,
        [Description("Arguments for the command")] string arguments)
    {
        ProcessStartInfo startInfo;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new NotImplementedException();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            throw new NotImplementedException();
        }
        else
        {
            arguments = $"{command} {arguments}";
            startInfo = new()
            {
                FileName = "dotnet",
                Arguments = arguments
            };
        }

        var result = await Task.Run(
            () => ProcessHelper.RunProcess(startInfo)
        );

        return result.Output;
    }
}