using Applique.LoadTester.Domain.Assembly;
using Applique.LoadTester.Domain.Service;

namespace Applique.LoadTester.Logic.Runtime.Engine
{
    public class ScenarioInstantiatorFactory
    {
        private readonly IBindingsFactory _bindingsFactory;
        private readonly StepInstantiatorFactory _stepInstantiatorFactory;

        public ScenarioInstantiatorFactory(
            IBindingsFactory bindingsFactory,
            StepInstantiatorFactory stepInstantiatorFactory)
        {
            _bindingsFactory = bindingsFactory;
            _stepInstantiatorFactory = stepInstantiatorFactory;
        }

        public IScenarioInstantiator Create(ITestSuite testSuite)
            => new ScenarioInstantiator(_bindingsFactory, _stepInstantiatorFactory, testSuite);
    }
}