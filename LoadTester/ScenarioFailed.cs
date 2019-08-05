using System;

namespace LoadTester
{
    public class ScenarioFailed : Exception
    {
        public ScenarioFailed(Step step, string message) : base($"Scenario failed on step {step.Endpoint} with error {message}")
        {
        }
    }
}