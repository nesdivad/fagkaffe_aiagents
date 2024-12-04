using Fagkaffe.CommandLine.Tools;
using Fagkaffe.Helpers;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Fagkaffe.Tools.Files;

[Description("Tool for reading and writing to files on the local system.")]
public static class FileTool
{
    [Description("Search for a file in the current directory, or children directories.")]
    public static async Task<string> SearchFileAsync(
        [Description("Name of file to search for")] string filename)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new NotImplementedException();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            throw new NotImplementedException();
        }

        ProcessStartInfo startInfo = new()
        {
            FileName = "find",
            Arguments = $". ! -name 'appsettings.json' ! -path '*.git*' ! -path '*bin*' ! -path '*obj*' -iname \"{filename}\""
        };

        var result = await Task.Run(() => ProcessHelper.RunProcess(startInfo));

        return result.Output ?? string.Empty;
    }

    [Description("Read the contents of a file from the supplied file path.")]
    public static async Task<string?> ReadFileAsync(
        [Description("Path of file to read from")] string filepath)
    {
        return await FileHelper.ReadFileAsync(filepath);
    }

    [Description("Write contents to a file. Best to use when writing new files or adding contents.")]
    public static async Task<string> WriteFileAsync(
        [Description("Path of file to write to")] string filepath,
        [Description("Contents to write")] string contents)
    {
        return await FileHelper.WriteFileAsync(filepath, contents);
    }

    [Description("""
        Replace text in a file.
        Returns: True if text is replaced, false if an error occurred.
    """)]
    public async static Task<bool> ReplaceTextAsync(
        [Description("The path to the file")] string filepath,
        [Description("The old text to be replaced")] string oldText,
        [Description("The new text to replace the old one")] string newText,
        [Description("True if all occurrences should be replaced")] bool replaceAllOccurrences = false)
    {
        string arguments = string.Empty;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var searchTerm = $"s/{oldText.EscapeChars()}/{newText.EscapeChars()}/";
            if (replaceAllOccurrences)
                searchTerm += 'g';

            arguments = $"-i {Constants.SedBackupExtension} -e \"{searchTerm}\" {filepath}";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            throw new NotImplementedException("Not yet implemented");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new NotImplementedException("Not yet implemented");
        }

        ProcessStartInfo startInfo = new()
        {
            FileName = "sed",
            Arguments = arguments
        };

        try
        {
            var result = await Task.Run(() => ProcessHelper.RunProcess(startInfo));
            return result.Output is null || !result.Output.Contains("invalid command code");
        }
        catch (Exception)
        {
            return false;
        }
    }

    [Description("Reverse changes to file, and returns the contents after reverse has been written to file")]
    public static async Task<string> ReverseFileChangesAsync(
        [Description("Name of file")] string filename)
    {
        var backupFilename = $"{filename}{Constants.SedBackupExtension}";

        var original = await SearchFileAsync(filename);
        var backup = await SearchFileAsync(backupFilename);

        if (string.IsNullOrEmpty(original))
            return "unable to locate original file";

        if (string.IsNullOrEmpty(backup))
            return "unable to locate backup file";

        try
        {
            var backupContents = await ReadFileAsync(backup)
                ?? throw new ArgumentNullException(paramName: backup);

            await WriteFileAsync(original, backupContents);
            return backupContents;
        }
        catch (Exception)
        {
            return "an error occurred while trying to reverse changes";
        }
    }

    [Description("Search for text in files. Returns filename, path, line number and matched text")]
    public static string SearchTextInFiles(
        [Description("The search term")]
        string searchTerm,
        [Description("The file(s) to include in search, e.g. *.cs")]
        string filesToInclude,
        [Description("Search recursively in directories")]
        bool recursive = true,
        [Description("Ignore case in text")]
        bool ignoreCase = true)
    {
        var workingDirectory = ".";
        var results = Grep.RetrieveFiles(
            searchTerm,
            workingDirectory,
            filesToInclude,
            recursive,
            ignoreCase
        );

        StringBuilder sb = new();
        foreach (var result in results)
        {
            sb.AppendLine($"{result.File}:{result.Number}:{result.Text}");
        }

        return sb.ToString();
    }
}