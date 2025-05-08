
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinariesCleaner;

internal static class Utils
{
    public static string[] ParseCmdArguments(string rawInput)
    {
        List<string> arguments = [];
        StringBuilder currentArgument = new();

        bool inQuotes = false;
        for (int i = 0; i < rawInput.Length; i++)
        {
            char currentChar = rawInput[i];

            if (currentChar == '\"')
            {
                inQuotes = !inQuotes;
            }
            else if (char.IsWhiteSpace(currentChar) && !inQuotes)
            {
                if (currentArgument.Length > 0)
                {
                    arguments.Add(currentArgument.ToString());
                    currentArgument.Clear();
                }
            }
            else
            {
                currentArgument.Append(currentChar);
            }
        }

        if (currentArgument.Length > 0)
        {
            arguments.Add(currentArgument.ToString());
        }

        return [.. arguments];
    }

    public static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static bool AskQuestion(string question)
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
}
