using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Fagkaffe.Helpers;

namespace Fagkaffe.Tools.Git;

[Description("Tools for running git commands")]
public static class GitTool
{
    [Description("Run git commands")]
    public static async Task<string?> GitAsync(
        [Description("the command to execute")]string command, 
        [Description("argumens for the command")]string arguments)
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
                FileName = "git",
                Arguments = arguments
            };
        }

        var result = await Task.Run(
            () => ProcessHelper.RunProcess(startInfo)
        );

        return result.Output;
    }
}