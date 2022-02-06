using Applique.LoadTester.Domain.Assembly;
using Applique.LoadTester.Domain.Service;

namespace Applique.LoadTester.Logic.Runtime.Engine
{
    public class ScenarioRunnerFactory : IScenarioRunnerFactory
    {
        private readonly IBindingsRepositoryFactory _bindingsRepositoryFactory;
        private readonly ScenarioInstantiatorFactory _scenarioInstantiatorFactory;

        public ScenarioRunnerFactory(
            IBindingsRepositoryFactory bindingsRepositoryFactory,
            ScenarioInstantiatorFactory scenarioInstantiatorFactory)
        {
            _bindingsRepositoryFactory = bindingsRepositoryFactory;
            _scenarioInstantiatorFactory = scenarioInstantiatorFactory;
        }

        public IScenarioRunner Create(ITestSuite testSuite)
            => new ScenarioRunner(
                _bindingsRepositoryFactory.Create(testSuite),
                _scenarioInstantiatorFactory.Create(testSuite));
    }
}