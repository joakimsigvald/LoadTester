using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;

namespace Applique.LoadTester.Runtime.Engine
{
    public class ScenarioRunnerFactory : IScenarioRunnerFactory
    {
        private readonly IBindingsFactory _bindingsFactory;
        private readonly IBindingsRepositoryFactory _bindingsRepositoryFactory;
        private readonly StepInstantiatorFactory _stepInstantiatorFactory;

        public ScenarioRunnerFactory(
            IBindingsFactory bindingsFactory,
            IBindingsRepositoryFactory bindingsRepositoryFactory,
            StepInstantiatorFactory stepInstantiatorFactory)
        {
            _bindingsFactory = bindingsFactory;
            _bindingsRepositoryFactory = bindingsRepositoryFactory;
            _stepInstantiatorFactory = stepInstantiatorFactory;
        }

        public IScenarioRunner Create(ITestSuite testSuite)
            => new ScenarioRunner(
                _bindingsFactory, 
                testSuite, 
                _bindingsRepositoryFactory.Create(testSuite), 
                _stepInstantiatorFactory);
    }
}