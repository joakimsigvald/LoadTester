using Applique.LoadTester.Domain;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Engine;
using Applique.LoadTester.Domain.Environment;

namespace Applique.LoadTester.Assembly
{
    public class Assembler : IAssembler
    {
        private readonly IFileSystem _fileSystem;

        public Assembler(IFileSystem fileSystem) => _fileSystem = fileSystem;

        public ITestSuite ReadTestSuite(string filename) => _fileSystem.Read<TestSuite>(filename);

        public Constant[] LoadConstants<T>(string filename) => _fileSystem.Read<Constant[]>(filename);
    }
}