using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Result;
using Applique.LoadTester.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Applique.LoadTester.Front
{
    public class TestResultPresenter
    {
        private const int _columnWidth = 15;

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
            => new[] { PresentCaptions(), "".PadRight(_columnWidth * 7, '-') };

        public static string PresentCaptions()
            => "Name".PadRight(2 * _columnWidth - 2) + "  "
                + "Count".PadRight(_columnWidth)
                + "Min".PadRight(_columnWidth)
                + "Max".PadRight(_columnWidth)
                + "Mean".PadRight(_columnWidth)
                + "Q75".PadRight(_columnWidth)
                + "Q90";

        public static IEnumerable<string> PresentBody(TestSuiteResult result)
            => result.ScenarioResults.SelectMany(Print);

        private static string PresentFooter()
            => "".PadRight(90, '-');

        private static IEnumerable<string> Print(IScenarioResult res)
            => res.Success ? PrintSuccessful(res) : PrintFailed(res);

        private static IEnumerable<string> PrintSuccessful(IScenarioResult res)
            => PrintExecutionTimes(res)
            .Concat(PrintStepResults(res.StepResults))
            .Concat(PrintBindings(res.Bindings));

        private static IEnumerable<string> PrintFailed(IScenarioResult res)
            => PrintFailedScenario(res)
            .Concat(PrintBindings(res.Bindings));

        private static IEnumerable<string> PrintFailedScenario(IScenarioResult res)
        {
            yield return ScenarioDivider;
            yield return PrintScenarioName(res);
            yield return res.Error;
        }

        private static IEnumerable<string> PrintExecutionTimes(IScenarioResult res)
        {
            yield return ScenarioDivider;
            yield return PrintScenarioName(res)
                + res.Scenario.Instances.ToString().PadRight(_columnWidth)
                + Print(res.Min)
                + Print(res.Max)
                + Print(res.Mean)
                + Print(res.Q75)
                + Print(res.Q90);
        }

        private static IEnumerable<string> PrintStepResults(IStepResult[] results)
        {
            foreach (var stepResult in results)
                yield return Print(stepResult);
        }

        private static IEnumerable<string> PrintBindings(IBindings bindings)
        {
            if (!bindings.Any())
                yield return "No bindings";
            yield return "-- BINDINGS --";
            foreach (var binding in bindings)
                yield return Print(binding);
        }

        private static string ScenarioDivider => "===========================";

        private static string Print(Constant constant)
            => $"| {constant.Name} = {constant.Value}";

        private static string PrintScenarioName(IScenarioResult res) => res.Scenario.Name.PadRight(2 * _columnWidth - 2) + "  ";

        private static string Print(IStepResult res)
            => $"  ->{res.Step.Endpoint}".PadRight(2 * _columnWidth - 2) + "  "
                + "".PadRight(_columnWidth)
                + Print(res.Min)
                + Print(res.Max)
                + Print(res.Mean)
                + Print(res.Q75)
                + Print(res.Q90);

        private static string Print(TimeSpan duration)
            => $"{duration.TotalSeconds:0.####} sec".PadRight(_columnWidth);
    }
}