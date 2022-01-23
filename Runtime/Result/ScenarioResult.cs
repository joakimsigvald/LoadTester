using Applique.LoadTester.Runtime.Environment;
using Applique.LoadTester.Design;
using System;
using System.Linq;

namespace Applique.LoadTester.Runtime.Result
{
    public class ScenarioResult
    {
        public static ScenarioResult Succeeded(Scenario scenario, ScenarioInstanceResult[] orderedResults)
        {
            var lastResult = orderedResults.Last();
            var durations = orderedResults.Select(or => or.Duration).ToArray();
            return new ScenarioResult
            {
                Scenario = scenario,
                Success = true,
                Max = durations.Last(),
                Min = durations.First(),
                Mean = GetQuantile(durations, 0.5f),
                Q75 = GetQuantile(durations, 0.75f),
                Q90 = GetQuantile(durations, 0.9f),
                StepResults = GetStepResults(scenario, orderedResults),
                AssertResults = lastResult.AssertResults.ToArray(),
                Bindings = lastResult.Bindings
            };
        }

        public static ScenarioResult Failed(Scenario scenario, ScenarioInstanceResult failedRun)
            => new()
            {
                Scenario = scenario,
                Error = failedRun.Error,
                Bindings = failedRun.Bindings
            };

        private static StepResult[] GetStepResults(Scenario scenario, ScenarioInstanceResult[] results)
        => results
            .SelectMany(res => res.StepTimes.Select((st, i) => (step: scenario.Steps[i], elapsed: st)))
            .GroupBy(pair => pair.step)
            .Select(g => new StepResult(g.Key, g.Select(pair => pair.elapsed).OrderBy(ts => ts).ToArray()))
            .ToArray();

        private static TimeSpan GetQuantile(TimeSpan[] orderedDurations, float quantile)
            => orderedDurations.Skip((int)(orderedDurations.Length * quantile)).First();

        private ScenarioResult() { }

        public Scenario Scenario { get; private set; }
        public Bindings Bindings { get; private set; }
        public string Error { get; private set; }
        public AssertResult[] AssertResults { get; private set; }
        public bool Success { get; private set; }
        public TimeSpan Min { get; private set; }
        public TimeSpan Max { get; private set; }
        public TimeSpan Mean { get; private set; }
        public TimeSpan Q75 { get; private set; }
        public TimeSpan Q90 { get; private set; }
        public StepResult[] StepResults { get; internal set; }
    }
}