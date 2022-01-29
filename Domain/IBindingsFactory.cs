using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Service;

namespace Applique.LoadTester.Domain
{
    public interface IBindingsFactory
    {
        IBindings CreateInstanceBindings(ITestSuite testSuite, IScenario scenario, Model[] models, int instanceId);
        IBindings CreateBindings(ITestSuite testSuite, Constant[] constants, Model[] models = null);
    }
}