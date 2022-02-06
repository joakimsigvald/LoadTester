using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Assembly;

namespace Applique.LoadTester.Domain.Service
{
    public interface IBindingsFactory
    {
        IBindings CreateInstanceBindings(ITestSuite testSuite, IScenario scenario, int instanceId);
        IBindings CreateBindings(ITestSuite testSuite, Constant[] constants);
    }
}