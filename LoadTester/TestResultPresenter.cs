using System;
using System.Collections.Generic;
using System.Linq;

namespace LoadTester
{
    public class TestResultPresenter
    {
        private const int ColumnWidth = 15;

        public static string[] PresentResult(string name, TestSuiteResult result)
            => PresentTitle(name, result)
            .Concat(PresentHeader())
            .Concat(PresentBody(result))
            .Append(PresentFooter())
            .ToArray();

        public static IEnumerable<string> PresentTitle(string name, TestSuiteResult result)
            => new[] {
                $"{name} " + (result.Success ? "succeeded" : "failed"),
                string.Empty
            };

        public static IEnumerable<string> PresentHeader()
            => new[] { PresentCaptions(), "".PadRight(ColumnWidth * 7, '-') };

        public static string PresentCaptions()
            => "Name".PadRight(2 * ColumnWidth - 2) + "  "
                + "Count".PadRight(ColumnWidth)
                + "Min".PadRight(ColumnWidth)
                + "Max".PadRight(ColumnWidth)
                + "Mean".PadRight(ColumnWidth)
                + "Q75".PadRight(ColumnWidth)
                + "Q90";

        public static IEnumerable<string> PresentBody(TestSuiteResult result)
            => result.ScenarioResults.SelectMany(Print);

        private static string PresentFooter()
            => "".PadRight(90, '-');

        private static IEnumerable<string> Print(ScenarioResult res)
            => res.Success ? PrintSuccessful(res) : PrintFailed(res);

        private static IEnumerable<string> PrintSuccessful(ScenarioResult res)
        {
            yield return PrintScenarioName(res)
                + res.Scenario.Instances.ToString().PadRight(ColumnWidth)
                + Print(res.Min)
                + Print(res.Max)
                + Print(res.Mean)
                + Print(res.Q75)
                + Print(res.Q90);
            foreach (var stepResult in res.StepResults)
                yield return Print(stepResult);
        }

        private static IEnumerable<string> PrintFailed(ScenarioResult res)
        {
            yield return PrintScenarioName(res) + res.Error;
        }

        private static string PrintScenarioName(ScenarioResult res) => res.Scenario.Name.PadRight((2 * ColumnWidth) - 2) + "  ";

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