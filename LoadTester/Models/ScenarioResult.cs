using System;
using System.Linq;

namespace LoadTester
{
    public class ScenarioResult
    {
        public static ScenarioResult Failed(Scenario scenario, string error)
            => new ScenarioResult
            {
                Scenario = scenario,
                Error = error
            };

        public static ScenarioResult Succeeded(Scenario scenario, TimeSpan[] orderedDurations)
            => new ScenarioResult
            {
                Scenario = scenario,
                Success = true,
                Max = orderedDurations.Last(),
                Min = orderedDurations.First(),
                Mean = GetQuantile(orderedDurations, 0.5f),
                Q75 = GetQuantile(orderedDurations, 0.75f),
                Q90 = GetQuantile(orderedDurations, 0.9f)
            };

        private static TimeSpan GetQuantile(TimeSpan[] orderedDurations, float quantile)
            => orderedDurations.Skip((int)(orderedDurations.Length * quantile)).First();

        private ScenarioResult() { }

        public Scenario Scenario { get; private set; }
        public string Error { get; private set; }
        public bool Success { get; private set; }
        public TimeSpan Min { get; private set; }
        public TimeSpan Max { get; private set; }
        public TimeSpan Mean { get; private set; }
        public TimeSpan Q75 { get; private set; }
        public TimeSpan Q90 { get; private set; }
    }
}