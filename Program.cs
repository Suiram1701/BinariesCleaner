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

        string? projectFile = Directory.GetFiles(directory).FirstOrDefault(file => file.EndsWith(".csproj"));
        if (!string.IsNullOrEmpty(projectFile))
        {
            PrintSuccess($"Project file found '{projectFile}'");

            string objPath = Path.Combine(directory, "obj");
            if (Directory.Exists(objPath))
            {
                removedFolders++;
                Directory.Delete(objPath, recursive: true);
                Console.WriteLine($"Removed '{objPath}'");
            }

            string binPath = Path.Combine(directory, "bin");
            if (Directory.Exists(binPath))
            {
                removedFolders++;
                Directory.Delete(binPath, recursive: true);
                Console.WriteLine($"Removed '{binPath}'");
            }

            Console.WriteLine("---------------------------------------------------------------------------");
        }

        foreach (string subDirectory in Directory.GetDirectories(directory))
        {
            removedFolders += RemoveRecursively(subDirectory);
        }
        return removedFolders;
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