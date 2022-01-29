using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Service;

namespace Applique.LoadTester.Assembly
{
    public class Assembler : IAssembler
    {
        private readonly IFileSystem _fileSystem;
        private readonly IScenarioRunnerFactory _scenarioRunnerFactory;

        public Assembler(IFileSystem fileSystem, IScenarioRunnerFactory scenarioRunnerFactory)
        {
            _fileSystem = fileSystem;
            _scenarioRunnerFactory = scenarioRunnerFactory;
        }

        public ITestSuiteRunner AssembleTestSuite(string filename)
        {
            var testSuite = _fileSystem.Read<TestSuite>(filename);
            return new TestSuiteRunner(_scenarioRunnerFactory, testSuite);
        }
    }
}