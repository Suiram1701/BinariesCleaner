using System.CommandLine;
using System.CommandLine.Parsing;

namespace BinariesCleaner;

internal class Program
{
    static int Main(string[] args)
    {
        Argument<DirectoryInfo> dirArgument = new(
            name: "Directory",
            description: "The root directory to start from with the clean up process. By default the working directory will be used.",
            getDefaultValue: () => new(Environment.CurrentDirectory));

        RootCommand root = new(description: "Cleans up binaries from .NET projects.");
        root.AddArgument(dirArgument);
        root.SetHandler(HandleRoot, dirArgument);

        return root.Invoke(args);
    }

    private static void HandleRoot(DirectoryInfo directory)
    {
        int removedFolders = 0;
        long removedBytes = 0;

        // .NET projects
        foreach (FileInfo projectFile in directory.EnumerateFiles("*.csproj", SearchOption.AllDirectories))
        {
            PrintSuccess($"Project file found '{projectFile}'");

            try
            {
                DirectoryInfo? objDir = TryGetDirectory(projectFile.Directory!, "obj");
                DirectoryInfo? binDir = TryGetDirectory(projectFile.Directory!, "bin"); 
                if (!((objDir?.Exists ?? false) || (binDir?.Exists ?? false)))
                {
                    Console.WriteLine("Skipped: No binaries to remove.");
                    continue;
                }

                long size = GetDirectorySize(objDir) + GetDirectorySize(binDir);
                if (AskQuestion($"Should the binaries of project \"{projectFile}\" be removed (size {size / 1000} Kb)?"))
                {
                    removedFolders += TryRemoveDirectory(objDir);
                    removedFolders += TryRemoveDirectory(binDir);
                    removedBytes += size;
                }
            }
            catch (Exception ex)
            {
                PrintError($"An error occurred while removing the binaries of \"{projectFile}\"!:\n{ex}");
            }

            Console.WriteLine("---------------------------------------------------------------------------");
        }

        // node packages
        foreach (FileInfo packageFile in directory.EnumerateFiles("package.json", SearchOption.AllDirectories))
        {
            PrintSuccess($"Package file found '{packageFile}'");

            try
            {
                DirectoryInfo? modulesDir = TryGetDirectory(packageFile.Directory!, "node_modules");
                if (!(modulesDir?.Exists ?? false))
                {
                    Console.WriteLine("Skipped: No modules to remove.");
                    continue;
                }

                long size = GetDirectorySize(modulesDir);
                if (AskQuestion($"Should the module files of package \"{packageFile}\" be removed (size {size / 1000} Kb)?"))
                {
                    removedFolders += TryRemoveDirectory(modulesDir);
                    removedBytes += size;
                }
            }
            catch (Exception ex)
            {
                PrintError($"An error occurred while removing the modules of \"{packageFile}\"!:\n{ex}");
            }
        }

        PrintSuccess($"Finished. {removedFolders} folders containing {removedBytes / 1000} Kb!");
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

    private static bool AskQuestion(string question)
    {
        Console.Write(question);
        Console.Write(" [y/n]: ");

        while (true)
        {
            string? input = Console.ReadLine();
            if (input?.Equals("Y", StringComparison.InvariantCultureIgnoreCase) ?? false)
                return true;
            else if (input?.Equals("N", StringComparison.InvariantCultureIgnoreCase) ?? false)
                return false;

            Console.WriteLine("Please input y or n to make the decision!");
        }
    }

    private static DirectoryInfo? TryGetDirectory(DirectoryInfo parent, string name)
    {
        string dir = Path.Combine(parent.FullName, name);
        return Directory.Exists(dir)
            ? new DirectoryInfo(dir)
            : null;
    }

    private static long GetDirectorySize(DirectoryInfo? dir)
    {
        return dir?
            .EnumerateFiles("", SearchOption.AllDirectories)
            .Sum(f => f.Length) ?? 0L;
    }

    private static int TryRemoveDirectory(DirectoryInfo? dir)
    {
        if (dir?.Exists ?? false)
        {
            dir.Delete(recursive: true);
            Console.WriteLine($"Removed '{dir}'");
            return 1;
        }

        return 0;
    }
}