using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Engine;

namespace Applique.LoadTester.Domain
{
    public interface IScenarioRunnerFactory
    {
        IScenarioRunner Create(ITestSuite testSuite);
    }
}