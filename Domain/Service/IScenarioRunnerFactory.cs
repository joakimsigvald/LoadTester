using Applique.LoadTester.Domain.Design;

namespace Applique.LoadTester.Domain.Service
{
    public interface IScenarioRunnerFactory
    {
        IScenarioRunner Create(ITestSuite testSuite);
    }
}