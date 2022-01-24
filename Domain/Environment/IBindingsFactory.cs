using Applique.LoadTester.Domain.Design;

namespace Applique.LoadTester.Domain.Environment
{
    public interface IBindingsFactory
    {
        IBindings CreateInstanceBindings(ITestSuite testSuite, IScenario scenario, Model[] models, int instanceId);
        IBindings CreateBindings(ITestSuite testSuite, Constant[] constants, Model[] models);
    }
}