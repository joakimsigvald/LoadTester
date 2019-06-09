using System;

namespace LoadTester
{
    public class ScenarioInstanceResult
    {
        public static ScenarioInstanceResult Succeeded(TimeSpan duration) => new ScenarioInstanceResult
        {
            Success = true,
            Duration = duration
        };

        public static ScenarioInstanceResult Failed(string error) => new ScenarioInstanceResult
        {
            Error = error
        };

        public bool Success { get; set; }
        public TimeSpan Duration { get; set; }
        public string Error { get; set; }
    }
}