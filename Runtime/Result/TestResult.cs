using System.Linq;

namespace Applique.LoadTester.Runtime.Result
{
    public class TestSuiteResult
    {
        public ScenarioResult[] ScenarioResults { get; set; }

        public TestSuiteResult(ScenarioResult[] scenarioResults)
        {
            ScenarioResults = scenarioResults;
        }

        public bool Success => ScenarioResults.All(sr => sr.Success);
    }
}