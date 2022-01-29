using Applique.LoadTester.Core.Design;

namespace Applique.LoadTester.Core.Service
{
    public interface IAssembler
    {
        ITestSuiteRunner AssembleTestSuite(string filename);
    }
}