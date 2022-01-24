using Applique.LoadTester.Domain.Environment;
using Applique.LoadTester.Runtime.Engine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Applique.LoadTester.Runtime.Result
{
    public class ScenarioInstanceResult
    {
        public static ScenarioInstanceResult Succeeded(RunnableScenario scenario, TimeSpan duration, IList<TimeSpan> stepTimes, AssertResult[] assertResults)
            => new()
            {
                Success = true,
                Duration = duration,
                StepTimes = stepTimes.ToArray(),
                AssertResults = assertResults,
                Bindings = scenario.Bindings
            };

        public static ScenarioInstanceResult Failed(RunnableScenario scenario, IEnumerable<AssertResult> failedResults)
            => Failed(scenario, $"Asserts failed: {string.Join(", ", failedResults.Select(fr => fr.Message))}");

        public static ScenarioInstanceResult Failed(RunnableScenario scenario, string error) => new()
        {
            Error = error,
            Bindings = scenario.Bindings
        };

        public bool Success { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan[] StepTimes { get; set; }
        public string Error { get; set; }
        public AssertResult[] AssertResults { get; set; }
        public IBindings Bindings { get; private set; }
    }
}