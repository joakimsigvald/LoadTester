using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;

namespace Applique.LoadTester.Runtime.Engine
{
    public class TestSuiteRunnerFactory : ITestSuiteRunnerFactory
    {
        private readonly IScenarioRunnerFactory _scenarioRunnerFactory;

        public TestSuiteRunnerFactory(IScenarioRunnerFactory scenarioRunnerFactory) 
            => _scenarioRunnerFactory = scenarioRunnerFactory;

        public ITestSuiteRunner Create(ITestSuite testSuite)
            => new TestSuiteRunner(_scenarioRunnerFactory, testSuite);
    }
}