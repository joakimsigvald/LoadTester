using Applique.LoadTester.Domain.Design;

namespace Applique.LoadTester.Domain.Environment
{
    public interface IFileSystem
    {
        bool Exists(string fileName);

        void Write(string fileName, object obj);

        void WriteLines(string fileName, string[] lines);

        TValue ReadValue<TValue>(string fileName) where TValue : struct;
        ITestSuite ReadTestSuite(string filename);
        Constant[] LoadConstants<T>(string bindingsPath);
    }
}
