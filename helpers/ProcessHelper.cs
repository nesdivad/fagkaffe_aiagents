using System.Diagnostics;

namespace Fagkaffe.Helpers;

public static class ProcessHelper
{
    public static ProcessResult RunProcess(ProcessStartInfo startInfo)
    {
        startInfo.CreateNoWindow = false;
        startInfo.RedirectStandardOutput = true;

        Process process = Process.Start(startInfo)!;
        process.WaitForExit(60000);
        var output = process.StandardOutput.ReadToEnd();

        return new(process.ExitCode, output);
    }
}

public class ProcessResult(int code, string? output)
{
    public int Code { get; } = code;
    public string? Output { get; } = output;
}