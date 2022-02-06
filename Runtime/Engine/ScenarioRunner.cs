using System.Linq;
using System.Threading.Tasks;
using Applique.LoadTester.Core.Result;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;
using Applique.LoadTester.Logic.Runtime.Result;

namespace Applique.LoadTester.Logic.Runtime.Engine
{
    public class ScenarioRunner : IScenarioRunner
    {
        private readonly IBindingsFactory _bindingsFactory;
        private readonly ITestSuite _testSuite;
        private readonly IBindingsRepository _bindingsRepository;
        private readonly StepInstantiatorFactory _stepInstantiatorFactory;

        public ScenarioRunner(
            IBindingsFactory bindingsFactory,
            ITestSuite testSuite,
            IBindingsRepository bindingsRepository,
            StepInstantiatorFactory stepInstantiatorFactory)
        {
            _bindingsFactory = bindingsFactory;
            _testSuite = testSuite;
            _bindingsRepository = bindingsRepository;
            _stepInstantiatorFactory = stepInstantiatorFactory;
        }

        public async Task<IScenarioResult> Run(IScenario scenario)
        {
            var instances = CreateRunnableScenarios(scenario);
            var runs = await Task.WhenAll(instances.Select(i => i.Run()));
            if (!runs.All(r => r.Success))
                return ScenarioResult.Failed(scenario, runs.First(r => !r.Success));
            if (scenario.Persist.Any())
                _bindingsRepository.PersistBindings(instances.Last().Bindings, scenario.Persist);
            return ScenarioResult.Succeeded(scenario,
                runs.OrderBy(d => d.Duration)
                .ToArray());
        }

        private RunnableScenario[] CreateRunnableScenarios(IScenario scenarioToRun)
        {
            var loadedBindings = _bindingsRepository.LoadBindings(scenarioToRun.Load);
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
        {
            var bindings = _bindingsFactory.CreateInstanceBindings(_testSuite, scenarioToRun, instanceId);
            StepInstantiator stepInstantiator = _stepInstantiatorFactory.Create(_testSuite, bindings);
            var steps = scenarioToRun.GetStepsToRun(_testSuite)
                .Select(stepInstantiator.Instanciate)
                .ToArray();
            return new(bindings, steps);
        }
    }
}