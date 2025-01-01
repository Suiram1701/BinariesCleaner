namespace BinariesCleaner;

internal class Program
{
    static void Main(string[] args)
    {
        string directory = args.FirstOrDefault(Environment.CurrentDirectory);
        if (!Directory.Exists(directory))
        {
            PrintError($"Unable to find target directory '{directory}'");
            return;
        }

        int removedFolders = RemoveRecursively(directory);
        PrintSuccess($"Finished. {removedFolders} folders containing binaries successfully removed");

        Console.ReadKey();
    }

    private static int RemoveRecursively(string directory)
    {
        int removedFolders = 0;

        string? projectFile = Directory.EnumerateFiles(directory).FirstOrDefault(file => file.EndsWith(".csproj"));
        if (!string.IsNullOrEmpty(projectFile))
        {
            PrintSuccess($"Project file found '{projectFile}'");

            string objPath = Path.Combine(directory, "obj");
            if (TryRemoveDirectory(objPath))
                removedFolders++;
            
            string binPath = Path.Combine(directory, "bin");
            if (TryRemoveDirectory(binPath))
                removedFolders++;

            Console.WriteLine("---------------------------------------------------------------------------");
        }

        string? packageFile = Directory.EnumerateFiles(directory).FirstOrDefault(file => Path.GetFileName(file) == "package.json");
        if (!string.IsNullOrEmpty(packageFile))
        {
            PrintSuccess($"Package file found '{packageFile}'");

            string modulesPath = Path.Combine(directory, "node_modules");
            if (TryRemoveDirectory(modulesPath))
                removedFolders++;

            Console.WriteLine("---------------------------------------------------------------------------");
        }

        foreach (string subDirectory in Directory.GetDirectories(directory))
        {
            removedFolders += RemoveRecursively(subDirectory);
        }
        return removedFolders;
    }

    private static bool TryRemoveDirectory(string directory)
    {
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, recursive: true);
            Console.WriteLine($"Removed '{directory}'");

            return true;
        }
        return false;
    }

    private static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.White;
    }

    private static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.White;
    }
}