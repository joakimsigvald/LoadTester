using System;
using System.Linq;
using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Result;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;

namespace Applique.LoadTester.Logic.Runtime.Result
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
                StepResults = GetStepResults(orderedResults),
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

        private static StepResult[] GetStepResults(ScenarioInstanceResult[] results)
        => results
            .SelectMany(res => res.StepDurations)
            .GroupBy(sd => sd.Step)
            .Select(g => new StepResult(g.Key.Endpoint, g.Select(sd => sd.Duration).OrderBy(ts => ts).ToArray()))
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