using System;
using System.Linq;

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
            Console.WriteLine("Name".PadRight(2 * ColumnWidth - 2) + "  " 
                + "Count".PadRight(ColumnWidth) 
                + "Min".PadRight(ColumnWidth) 
                + "Max".PadRight(ColumnWidth) 
                + "Mean".PadRight(ColumnWidth) 
                + "Q75".PadRight(ColumnWidth) 
                + "Q90");
            Console.WriteLine("".PadRight(ColumnWidth * 7, '-'));
            result.ScenarioResults.ToList().ForEach(Present);
            Console.WriteLine("".PadRight(90, '-'));
        }

        private static void Present(ScenarioResult res) {
            Console.WriteLine(Print(res));
            foreach (var stepResult in res.StepResults)
                Console.WriteLine(Print(stepResult));
        }

        private static string Print(ScenarioResult res)
            => res.Scenario.Name.PadRight(2 * ColumnWidth - 2) + "  "
                    + res.Scenario.Instances.ToString().PadRight(ColumnWidth)
                    + Print(res.Min)
                    + Print(res.Max)
                    + Print(res.Mean)
                    + Print(res.Q75)
                    + Print(res.Q90);

        private static string Print(StepResult res)
            => $"  ->{res.Step.Endpoint}".PadRight(2 * ColumnWidth - 2) + "  "
                    + "".PadRight(ColumnWidth)
                    + Print(res.Min)
                    + Print(res.Max)
                    + Print(res.Mean)
                    + Print(res.Q75)
                    + Print(res.Q90);

        private static string Print(TimeSpan duration)
            => $"{duration.TotalSeconds:0.####} sec".PadRight(ColumnWidth);
    }
}