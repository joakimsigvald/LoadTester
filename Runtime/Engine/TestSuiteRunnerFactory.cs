using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Assembly;
using Applique.LoadTester.Domain.Service;
using Applique.LoadTester.Runtime.Engine;

namespace Applique.LoadTester.Logic.Runtime.Engine
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