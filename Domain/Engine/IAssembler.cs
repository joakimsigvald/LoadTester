using Applique.LoadTester.Domain.Design;

namespace Applique.LoadTester.Domain.Engine
{
    public interface IAssembler
    {
        ITestSuite ReadTestSuite(string filename);
        Constant[] LoadConstants<T>(string filename);
    }
}