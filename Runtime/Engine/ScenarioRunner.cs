using System;
using System.Linq;
using System.Threading.Tasks;
using Applique.LoadTester.Core.Result;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Assembly;
using Applique.LoadTester.Domain.Service;
using Applique.LoadTester.Logic.Runtime.Result;

namespace Applique.LoadTester.Logic.Runtime.Engine;

public class ScenarioRunner : IScenarioRunner
{
    private readonly IBindingsRepository _bindingsRepository;
    private readonly IScenarioInstantiator _scenarioInstantiator;

    public ScenarioRunner(IBindingsRepository bindingsRepository, IScenarioInstantiator scenarioInstantiator)
    {
        _bindingsRepository = bindingsRepository;
        _scenarioInstantiator = scenarioInstantiator;
    }

    public async Task<IScenarioResult> Run(ITestSuiteScenario wrapper)
    {
        var scenario = wrapper.Scenario;
        Console.WriteLine("--------------------------");
        Console.WriteLine($"Running scenario: {scenario.Name} with {scenario.Instances} instances");

        var instances = CreateRunnableScenarios();
        var runs = await Task.WhenAll(instances.Select(i => i.Run()));
        if (!runs.All(r => r.Success))
            return ScenarioResult.Failed(scenario, runs.First(r => !r.Success));
        if (scenario.Persist.Any())
            _bindingsRepository.PersistBindings(instances.Last().Bindings, scenario.Persist);
        var instanceResults = runs.OrderBy(d => d.Duration).ToArray();
        return ScenarioResult.Succeeded(scenario, instanceResults);

        RunnableScenario[] CreateRunnableScenarios()
        {
            var loadedBindings = _bindingsRepository.LoadBindings(scenario.Load);
            return Enumerable.Range(1, scenario.Instances)
                .Select(i => CreateRunnableScenario(i, loadedBindings))
                .ToArray();
        }

        RunnableScenario CreateRunnableScenario(int i, IBindings loadedBindings)
        {
            var instance = _scenarioInstantiator.CreateInstance(wrapper, i);
            instance.Bindings.MergeWith(loadedBindings);
            return instance;
        }
    }
}