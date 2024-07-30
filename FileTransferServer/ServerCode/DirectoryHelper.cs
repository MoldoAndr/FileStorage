public static class DirectoryHelper
{
    public static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Console.WriteLine($"Created directory: {path}");
        }
    }

    public static bool HasWritePermission(string filePath)
    {
        try
        {
            using (FileStream fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                return true;
            }
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking write permissions: {ex.Message}");
            return false;
        }
    }
}
