using Applique.LoadTester.Core.Design;

namespace Applique.LoadTester.Core.Service
{
    public interface IScenarioRunnerFactory
    {
        IScenarioRunner Create(ITestSuite testSuite);
    }
}