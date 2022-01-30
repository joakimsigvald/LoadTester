using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Service;

namespace Applique.LoadTester.Assembly
{
    public class Assembler : IAssembler
    {
        private readonly IFileSystem _fileSystem;
        private readonly ITestSuiteRunnerFactory _testSuiteRunnerFactory;

        public Assembler(IFileSystem fileSystem, ITestSuiteRunnerFactory testSuiteRunnerFactory)
        {
            _fileSystem = fileSystem;
           _testSuiteRunnerFactory = testSuiteRunnerFactory;
        }

        public ITestSuiteRunner AssembleTestSuite(string filename)
        {
            var testSuite = _fileSystem.Read<TestSuite>(filename);
            return _testSuiteRunnerFactory.Create(testSuite);
        }
    }
}