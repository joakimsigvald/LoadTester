using System;
using System.Linq;
using System.Threading.Tasks;
using Applique.LoadTester.Runtime.Result;
using Applique.LoadTester.Runtime.External;
using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Result;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;

namespace Applique.LoadTester.Runtime.Engine
{
    public class ScenarioRunner : IScenarioRunner
    {
        private readonly IFileSystem _fileSystem;
        private readonly IRestCallerFactory _restCallerFactory;
        private readonly IBlobRepositoryFactory _blobRepositoryFactory;
        private readonly ITestSuite _testSuite;
        private readonly ILoader _loader;
        private readonly IBindingsFactory _bindingsFactory;
        private readonly IStepVerifierFactory _stepVerifierFactory;

        public ScenarioRunner(
            IFileSystem fileSystem,
            IRestCallerFactory restCallerFactory,
            IBlobRepositoryFactory blobRepositoryFactory,
            ITestSuite testSuite,
            ILoader loader,
            IBindingsFactory bindingsFactory,
            IStepVerifierFactory stepVerifierFactory)
        {
            _fileSystem = fileSystem;
            _restCallerFactory = restCallerFactory;
            _blobRepositoryFactory = blobRepositoryFactory;
            _testSuite = testSuite;
            _loader = loader;
            _bindingsFactory = bindingsFactory;
            _stepVerifierFactory = stepVerifierFactory;
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

        private RunnableScenario CreateRunnableScenario(IScenario scenarioToRun, int i, IBindings loadedBindings)
        {
            var instance = CreateInstance(scenarioToRun, i);
            instance.Bindings.MergeWith(loadedBindings);
            return instance;
        }

        private RunnableScenario CreateInstance(IScenario scenarioToRun, int instanceId)
            => new(
                _restCallerFactory, 
                _blobRepositoryFactory, 
                _testSuite, 
                scenarioToRun, 
                _bindingsFactory, 
                _stepVerifierFactory, 
                instanceId);

        private void PersistBindings(IBindings bindings, string[] propertiesToPersist)
            => _fileSystem.Write(BindingsPath, bindings
                .Join(propertiesToPersist, b => b.Name, p => p, (b, _) => b)
                .ToArray());

        private IBindings LoadBindings(string[] loadProperties)
        {
            if (!_fileSystem.Exists(BindingsPath) || !loadProperties.Any())
                return _bindingsFactory.CreateBindings(_testSuite, Array.Empty<Constant>(), Array.Empty<Model>());
            var constants = _loader.LoadConstants<Constant[]>(BindingsPath);
            var constantsToLoad = constants.Join(loadProperties, b => b.Name, p => p, (b, _) => b).ToArray();
            return _bindingsFactory.CreateBindings(_testSuite, constantsToLoad, Array.Empty<Model>());
        }

        private string BindingsPath => $"{_testSuite.Name}_Bindings";
    }
}