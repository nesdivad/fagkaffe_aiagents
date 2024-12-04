using System.Text.RegularExpressions;

namespace Fagkaffe.CommandLine.Tools;

public static partial class Grep
{
    private static readonly Regex Appsettings = AppsettingsRegex();
    private static readonly Regex Dll = DllRegex();
    private static readonly Regex ObjBin = ObjBinRegex();

    public static IList<GrepResult> RetrieveFiles(
        string regexSearchTerm,
        string workingDirectory,
        string regexFilesToInclude,
        bool recursive = true,
        bool ignoreCase = true)
    {
        var files = GetFilenames(workingDirectory, regexFilesToInclude, recursive);
        var result = Search(regexSearchTerm, files, ignoreCase);

        return result;
    }

    private static IEnumerable<string> GetFilenames(
        string workingDir,
        string includeFile,
        bool recursive)
    {
        SearchOption searchOption = recursive
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;

        var dirName = Path.GetDirectoryName(workingDir);
        if (string.IsNullOrWhiteSpace(dirName))
            dirName = ".";

        var files = Directory.EnumerateFiles(dirName, includeFile, searchOption)
            .Where(x => !Appsettings.IsMatch(x))
            .Where(x => !Dll.IsMatch(x))
            .Where(x => !ObjBin.IsMatch(x));

        return [.. files];
    }

    private static IList<GrepResult> Search(
        string searchPattern,
        IEnumerable<string> files,
        bool ignoreCase)
    {
        ThreadLocal<Regex> regex = new(
            () => new Regex(
                searchPattern,
                RegexOptions.Compiled | (ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None)
            )
        );

        try
        {
            ParallelQuery<GrepResult>? matches =
                from file in files.AsParallel()
                                  .AsOrdered()
                                  .WithMergeOptions(ParallelMergeOptions.NotBuffered)
                from line in File.ReadLines(file)
                                 .Zip(Enumerable.Range(1, int.MaxValue),
                                    (s, i) => new GrepResult(i, s, file))
                where regex.Value != null && regex.Value.IsMatch(line.Text)
                select line;

            return [.. matches];
        }
        catch (Exception)
        {
            return [];
        }
    }

    [GeneratedRegex(@"appsettings(\.[^.]+)?\.json$", RegexOptions.Compiled)]
    private static partial Regex AppsettingsRegex();

    [GeneratedRegex(@"\b\w+\.dll\b", RegexOptions.Compiled)]
    private static partial Regex DllRegex();

    [GeneratedRegex(@"^.*\/(obj|bin)\/.*\/[^\/]+$", RegexOptions.Compiled)]
    private static partial Regex ObjBinRegex();
}

public class GrepResult(int number, string text, string file)
{
    public int Number { get; } = number;
    public string Text { get; } = text;
    public string File { get; } = file;
}