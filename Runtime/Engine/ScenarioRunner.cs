using System;
using System.Linq;
using System.Threading.Tasks;
using Applique.LoadTester.Domain.Environment;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Result;
using Applique.LoadTester.Runtime.Result;
using Applique.LoadTester.Runtime.Environment;
using Applique.LoadTester.Domain.Engine;

namespace Applique.LoadTester.Runtime.Engine
{
    public class ScenarioRunner : IScenarioRunner
    {
        private readonly IFileSystem _fileSystem;
        private readonly IRestCallerFactory _restCallerFactory;
        private readonly IBlobRepositoryFactory _blobRepositoryFactory;
        private readonly ITestSuite _testSuite;

        public ScenarioRunner(
            IFileSystem fileSystem,
            IRestCallerFactory restCallerFactory,
            IBlobRepositoryFactory blobRepositoryFactory,
            ITestSuite testSuite)
        {
            _fileSystem = fileSystem;
            _restCallerFactory = restCallerFactory;
            _blobRepositoryFactory = blobRepositoryFactory;
            _testSuite = testSuite;
        }

        public async Task<IScenarioResult> Run(IScenario scenario)
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

        private IScenario GetScenarioToRun(IScenario scenario)
            => scenario.Template == null
            ? scenario
            : GetScenarioToRun(_testSuite.GetTemplate(scenario.Template)).MergeWith(scenario);

        private RunnableScenario[] CreateRunnableScenarios(IScenario scenarioToRun)
        {
            var loadedBindings = LoadBindings(scenarioToRun.Load);
            return Enumerable.Range(1, scenarioToRun.Instances)
                .Select(i => CreateRunnableScenario(scenarioToRun, i, loadedBindings))
                .ToArray();
        }

        private RunnableScenario CreateRunnableScenario(IScenario scenarioToRun, int i, Bindings loadedBindings)
        {
            var instance = CreateInstance(scenarioToRun, i);
            instance.Bindings.MergeWith(loadedBindings);
            return instance;
        }

        private RunnableScenario CreateInstance(IScenario scenarioToRun, int instanceId)
            => new(_fileSystem, _restCallerFactory, _blobRepositoryFactory, _testSuite, scenarioToRun, instanceId);

        private void PersistBindings(IBindings bindings, string[] propertiesToPersist)
            => _fileSystem.Write(BindingsPath, bindings
                .Join(propertiesToPersist, b => b.Name, p => p, (b, _) => b)
                .ToArray());

        private Bindings LoadBindings(string[] loadProperties)
        {
            if (!_fileSystem.Exists(BindingsPath) || !loadProperties.Any())
                return new Bindings(_fileSystem, _testSuite, Array.Empty<Constant>(), Array.Empty<Model>());
            var constants = _fileSystem.LoadConstants<Constant[]>(BindingsPath);
            var constantsToLoad = constants.Join(loadProperties, b => b.Name, p => p, (b, _) => b).ToArray();
            return new Bindings(_fileSystem, _testSuite, constantsToLoad, Array.Empty<Model>());
        }

        private string BindingsPath => $"{_testSuite.Name}_Bindings";
    }
}