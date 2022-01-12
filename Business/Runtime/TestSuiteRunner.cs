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
            var scenarioToRun = GetScenarioToRun(scenario);
            var instances = CreateRunnableScenarios(scenarioToRun);
            var runs = await Task.WhenAll(instances.Select(i => i.Run()));
            if (!runs.All(r => r.Success))
                return ScenarioResult.Failed(scenarioToRun, runs.First(r => !r.Success));
            if (scenarioToRun.Persist.Any())
                PersistBindings(instances.Last().Bindings, scenarioToRun.Persist);
            return ScenarioResult.Succeeded(scenarioToRun,
                runs.OrderBy(d => d.Duration)
                .ToArray());
        }

        private RunnableScenario[] CreateRunnableScenarios(Scenario scenarioToRun)
        {
            var loadedBindings = LoadBindings(scenarioToRun.Load);
            return Enumerable.Range(1, scenarioToRun.Instances)
                .Select(i => CreateRunnableScenario(scenarioToRun, i, loadedBindings))
                .ToArray();
        }

        private RunnableScenario CreateRunnableScenario(Scenario scenarioToRun, int i, Bindings loadedBindings)
        {
            var instance = CreateInstance(scenarioToRun, i);
            instance.Bindings.MergeWith(loadedBindings);
            return instance;
        }

        public RunnableScenario CreateInstance(Scenario scenarioToRun, int instanceId)
            => new(_fileSystem, _testSuite, scenarioToRun, instanceId);

        private Scenario GetScenarioToRun(Scenario scenario)
            => scenario.Template == null
            ? scenario
            : _testSuite.GetTemplate(scenario.Template).MergeWith(scenario);

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