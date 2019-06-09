using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LoadTester
{
    public class TestResultPresenter
    {
        private const int ColumnWidth = 15;

        public static void PresentResult(string name, TestSuiteResult result)
        {
            if (result.Success)
                PresentSuccessfulResult(name, result);
            else
                Console.WriteLine($"{name} failed");
        }

        private static void PresentSuccessfulResult(string name, TestSuiteResult result)
        {
            Console.WriteLine($"{name} was successful");
            Console.WriteLine();
            Console.WriteLine("Name".PadLeft(2 * ColumnWidth - 2) + "  " 
                + "Count".PadRight(ColumnWidth) 
                + "Min".PadRight(ColumnWidth) 
                + "Max".PadRight(ColumnWidth) 
                + "Mean".PadRight(ColumnWidth) 
                + "Q75".PadRight(ColumnWidth) 
                + "Q90");
            Console.WriteLine("".PadRight(ColumnWidth * 7, '-'));
            foreach (var res in result.ScenarioResults)
                Console.WriteLine(Print(res));
            Console.WriteLine("".PadRight(90, '-'));
        }

        private static string Print(ScenarioResult res)
            => res.Scenario.Name.PadLeft(2 * ColumnWidth - 2) + "  "
                    + res.Scenario.Instances.ToString().PadRight(ColumnWidth)
                    + Print(res.Min)
                    + Print(res.Max)
                    + Print(res.Mean)
                    + Print(res.Q75)
                    + Print(res.Q90);

        private static string Print(TimeSpan duration)
            => $"{duration.TotalSeconds:0.####} sec".PadRight(ColumnWidth);
    }
}