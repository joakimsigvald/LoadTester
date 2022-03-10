using System;

namespace Applique.LoadTester.Front;

public static class Interactor
{
    public static bool Ask(string question)
    {
        Console.WriteLine($"{question}? (Y/N)");
        var answer = Console.ReadKey();
        Console.WriteLine();
        return answer.Key == ConsoleKey.Y;
    }

    internal static T Get<T>(string request)
    {
        Console.WriteLine($"Input {request}");
        var answer = Console.ReadLine();
        Console.WriteLine();
        return (T)Convert.ChangeType(answer, typeof(T));
    }
}