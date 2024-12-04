using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Fagkaffe.Helpers;

namespace Fagkaffe.Tools.Dotnet;

[Description("Tools for running dotnet commands")]
public static class DotnetTool
{
    [Description("Run 'dotnet build' command on a specific .csproj file")]
    public static async Task<string?> BuildAsync(
        [Description("Path to project file. Needs to be of type .csproj")] string projectpath)
    {
        if (!projectpath.EndsWith(".csproj"))
            return "Wrong file type, please enter path to a .csproj-file";

        var arguments = string.Empty;
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
            arguments = $"build {projectpath} --no-restore --tl:on";
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