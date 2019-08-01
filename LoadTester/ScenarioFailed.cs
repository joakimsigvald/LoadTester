using System;

namespace LoadTester
{
    public class ScenarioFailed : Exception
    {
        public ScenarioFailed(string message) : base(message)
        {
        }
    }
}