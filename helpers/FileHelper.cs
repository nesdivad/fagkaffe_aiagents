namespace Fagkaffe.Helpers;

public static class FileHelper
{

    public static async Task<string?> ReadFileAsync(
        string filepath)
    {
        try
        {
            using StreamReader reader = new(filepath);
            return await reader.ReadToEndAsync();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static async Task<string> WriteFileAsync(
        string filepath,
        string contents)
    {
        try
        {
            using StreamWriter output = new(filepath);
            foreach (var line in contents.Split("\n"))
            {
                await output.WriteLineAsync(line);
            }

            return "file write successful";
        }
        catch (DirectoryNotFoundException)
        {
            return "directory not found";
        }
        catch (Exception)
        {
            return "an error occurred while trying to write to file";
        }
    }
}