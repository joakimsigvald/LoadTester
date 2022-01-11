using Applique.LoadTester.Business.Design;
using Applique.LoadTester.Business.External;
using Applique.LoadTester.Business.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Applique.LoadTester.Business.Runtime
{
    public class TestSuiteRunner
    {
        private readonly IFileSystem _fileSystem;
        private readonly TestSuite _testSuite;

        public TestSuiteRunner(IFileSystem fileSystem, TestSuite suite)
        {
            _fileSystem = fileSystem;
            _testSuite = suite;
        }

        public async Task<TestSuiteResult> Run()
        {
            var results = new List<ScenarioResult>();
            foreach (var scenario in _testSuite.RunnableScenarios)
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
            var instances = CreateRunnableScenarios(scenario);
            var runs = await Task.WhenAll(instances.Select(i => i.Run()));
            if (!runs.All(r => r.Success))
                return ScenarioResult.Failed(scenario, runs.First(r => !r.Success));
            if (scenario.Persist.Any())
                PersistBindings(instances.Last().Bindings, scenario.Persist);
            return ScenarioResult.Succeeded(scenario,
                runs.OrderBy(d => d.Duration)
                .ToArray());
        }

        private RunnableScenario[] CreateRunnableScenarios(Scenario scenario)
        {
            var loadedBindings = LoadBindings(scenario.Load);
            return Enumerable.Range(1, scenario.Instances)
                .Select(i => CreateRunnableScenario(scenario, i, loadedBindings))
                .ToArray();
        }

        private RunnableScenario CreateRunnableScenario(Scenario scenario, int i, Bindings loadedBindings)
        {
            var instance = scenario.CreateInstance(_fileSystem, _testSuite, i);
            instance.Bindings.MergeWith(loadedBindings);
            return instance;
        }

        private void PersistBindings(Bindings bindings, string[] propertiesToPersist)
            => _fileSystem.Write(BindingsPath, bindings
                .Join(propertiesToPersist, b => b.Name, p => p, (b, _) => b)
                .ToArray());

        private Bindings LoadBindings(string[] loadProperties)
        {
            if (!_fileSystem.Exists(BindingsPath) || !loadProperties.Any())
                return new Bindings(_fileSystem, _testSuite);
            var constants = _fileSystem.Read<Constant[]>(BindingsPath);
            var constantsToLoad = constants.Join(loadProperties, b => b.Name, p => p, (b, _) => b).ToArray();
            return new Bindings(_fileSystem, _testSuite, constantsToLoad);
        }

        private string BindingsPath => $"{_testSuite.Name}_Bindings";
    }
}