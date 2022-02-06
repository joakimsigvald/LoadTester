using System.Linq;
using Applique.LoadTester.Domain.Assembly;
using Applique.LoadTester.Domain.Service;

namespace Applique.LoadTester.Logic.Runtime.Engine
{
    public class ScenarioInstantiator : IScenarioInstantiator
    {
        private readonly IBindingsFactory _bindingsFactory;
        private readonly StepInstantiatorFactory _stepInstantiatorFactory;
        private readonly ITestSuite _testSuite;

        public ScenarioInstantiator(
            IBindingsFactory bindingsFactory,
            StepInstantiatorFactory stepInstantiatorFactory,
            ITestSuite testSuite)
        {
            _bindingsFactory = bindingsFactory;
            _stepInstantiatorFactory = stepInstantiatorFactory;
            _testSuite = testSuite;
        }

        public RunnableScenario CreateInstance(ITestSuiteScenario wrapper, int instanceId)
        {
            var bindings = _bindingsFactory.CreateInstanceBindings(_testSuite, wrapper.Scenario, instanceId);
            StepInstantiator stepInstantiator = _stepInstantiatorFactory.Create(_testSuite, bindings);
            var steps = wrapper.GetStepsToRun().Select(stepInstantiator.Instanciate).ToArray();
            return new(bindings, steps);
        }
    }
}