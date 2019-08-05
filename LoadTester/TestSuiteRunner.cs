using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoadTester
{
    public class TestSuiteRunner
    {
        private readonly TestSuite _suite;

        public TestSuiteRunner(TestSuite suite)
        {
            _suite = suite;
        }

        public async Task<TestSuiteResult> Run()
        {
            var results = new List<ScenarioResult>();
            foreach (var scenario in _suite.Scenarios)
            {
                Console.WriteLine("Running scenario: " + scenario.Name);
                var result = await Run(scenario);
                results.Add(result);
                if (result.Success)
                {
                    Console.WriteLine("Successfully completed scenario");
                }
                else
                {
                    Console.WriteLine("Scenario failed, because: " + result.Error);
                    break;
                }
            }
            return new TestSuiteResult(results.ToArray());
        }

        public async Task<ScenarioResult> Run(Scenario scenario)
        {
            var instances = Enumerable.Range(1, scenario.Instances)
                .Select(i => scenario.CreateInstance(_suite, i))
                .ToArray();
            var runs = await Task.WhenAll(instances.Select(i => i.Run()));
            if (!runs.All(r => r.Success))
                return ScenarioResult.Failed(scenario, string.Join(", ", runs.Where(r => !r.Success).Select(r => r.Error)));
            return ScenarioResult.Succeeded(scenario,
                runs.OrderBy(d => d.Duration)
                .ToArray());
        }
    }
}
