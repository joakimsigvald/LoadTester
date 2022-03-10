using Applique.LoadTester.Domain.Assembly;

namespace Applique.LoadTester.Domain.Service;

public interface IScenarioRunnerFactory
{
    IScenarioRunner Create(ITestSuite testSuite);
}