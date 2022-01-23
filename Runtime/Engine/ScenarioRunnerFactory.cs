using Applique.LoadTester.Domain;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Engine;
using Applique.LoadTester.Domain.Environment;

namespace Applique.LoadTester.Runtime.Engine
{
    public class ScenarioRunnerFactory : IScenarioRunnerFactory
    {
        private readonly IFileSystem _fileSystem;
        private readonly IRestCallerFactory _restCallerFactory;
        private readonly IBlobRepositoryFactory _blobRepositoryFactory;

        public ScenarioRunnerFactory(IFileSystem fileSystem, IRestCallerFactory restCallerFactory, IBlobRepositoryFactory blobRepositoryFactory)
        {
            _fileSystem = fileSystem;
            _restCallerFactory = restCallerFactory;
            _blobRepositoryFactory = blobRepositoryFactory;
        }

        public IScenarioRunner Create(ITestSuite testSuite)
            => new ScenarioRunner(_fileSystem, _restCallerFactory, _blobRepositoryFactory, testSuite);
    }
}