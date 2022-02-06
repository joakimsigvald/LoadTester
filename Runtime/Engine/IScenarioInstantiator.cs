using Applique.LoadTester.Domain.Assembly;

namespace Applique.LoadTester.Logic.Runtime.Engine
{
    public interface IScenarioInstantiator
    {
        RunnableScenario CreateInstance(ITestSuiteScenario wrapper, int instanceId);
    }
}