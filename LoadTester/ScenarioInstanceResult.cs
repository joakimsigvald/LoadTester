using System;
using System.Collections.Generic;
using System.Linq;

namespace LoadTester
{
    public class ScenarioInstanceResult
    {
        public static ScenarioInstanceResult Succeeded(TimeSpan duration, IList<TimeSpan> stepTimes) => new ScenarioInstanceResult
        {
            Success = true,
            Duration = duration,
            StepTimes = stepTimes.ToArray()
        };

        public static ScenarioInstanceResult Failed(string error) => new ScenarioInstanceResult
        {
            Error = error
        };

        public bool Success { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan[] StepTimes { get; set; }
        public string Error { get; set; }
    }
}