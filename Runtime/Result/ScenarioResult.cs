using System;
using System.Linq;
using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Result;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;

namespace Applique.LoadTester.Runtime.Result
{
    public class ScenarioResult : IScenarioResult
    {
        public static ScenarioResult Succeeded(IScenario scenario, ScenarioInstanceResult[] orderedResults)
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
                Bindings = lastResult.Bindings
            };
        }

        public static ScenarioResult Failed(IScenario scenario, ScenarioInstanceResult failedRun)
            => new()
            {
                Scenario = scenario,
                Error = failedRun.Error,
                Bindings = failedRun.Bindings
            };

        private static StepResult[] GetStepResults(IScenario scenario, ScenarioInstanceResult[] results)
        => results
            .SelectMany(res => res.StepTimes.Select((st, i) => (step: scenario.Steps[i], elapsed: st)))
            .GroupBy(pair => pair.step)
            .Select(g => new StepResult(g.Key, g.Select(pair => pair.elapsed).OrderBy(ts => ts).ToArray()))
            .ToArray();

        private static TimeSpan GetQuantile(TimeSpan[] orderedDurations, float quantile)
            => orderedDurations.Skip((int)(orderedDurations.Length * quantile)).First();

        private ScenarioResult() { }

        public IScenarioMetadata Scenario { get; private set; }
        public IStepResult[] StepResults { get; internal set; }
        public IBindings Bindings { get; private set; }
        public string Error { get; private set; }
        public bool Success { get; private set; }
        public TimeSpan Min { get; private set; }
        public TimeSpan Max { get; private set; }
        public TimeSpan Mean { get; private set; }
        public TimeSpan Q75 { get; private set; }
        public TimeSpan Q90 { get; private set; }
    }
}