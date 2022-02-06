using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Assembly;

namespace Applique.LoadTester.Domain.Service
{
    public interface ITestSuiteRunnerFactory
    {
        ITestSuiteRunner Create(ITestSuite testSuite);
    }
}