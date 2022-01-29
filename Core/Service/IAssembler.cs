using Applique.LoadTester.Core.Design;

namespace Applique.LoadTester.Core.Service
{
    public interface IAssembler
    {
        ITestSuite ReadTestSuite(string filename);
        Constant[] LoadConstants<T>(string filename);
    }
}