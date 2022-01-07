using Applique.LoadTester.Business.Design;
using Applique.LoadTester.Business.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Applique.LoadTester.Business.Runtime
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
            foreach (var scenario in _suite.RunnableScenarios)
            {
                Console.WriteLine("--------------------------");
                Console.WriteLine($"Running scenario: {scenario.Name} with {scenario.Instances} instances");
                var result = await Run(scenario);
                results.Add(result);
                Console.WriteLine("Scenario " + (result.Success ? "succeeded" : "failed"));
            }
            return new TestSuiteResult(results.OrderByDescending(res => res.Success).ToArray());
        }

        public async Task<ScenarioResult> Run(Scenario scenario)
        {
            var instances = Enumerable.Range(1, scenario.Instances)
                .Select(i => scenario.CreateInstance(_suite, i))
                .ToArray();
            var runs = await Task.WhenAll(instances.Select(i => i.Run()));
            if (!runs.All(r => r.Success))
                return ScenarioResult.Failed(scenario, runs.First(r => !r.Success));
            return ScenarioResult.Succeeded(scenario,
                runs.OrderBy(d => d.Duration)
                .ToArray());
        }
    }
}
