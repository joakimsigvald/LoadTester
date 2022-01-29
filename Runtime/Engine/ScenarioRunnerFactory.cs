using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;
using Applique.LoadTester.Runtime.External;

namespace Applique.LoadTester.Runtime.Engine
{
    public class ScenarioRunnerFactory : IScenarioRunnerFactory
    {
        private readonly IFileSystem _fileSystem;
        private readonly IRestCallerFactory _restCallerFactory;
        private readonly IBlobRepositoryFactory _blobRepositoryFactory;
        private readonly ILoader _loader;
        private readonly IBindingsFactory _bindingsFactory;
        private readonly IStepVerifierFactory _stepVerifierFactory;

        public ScenarioRunnerFactory(
            IFileSystem fileSystem, 
            IRestCallerFactory restCallerFactory, 
            IBlobRepositoryFactory blobRepositoryFactory,
            ILoader loader,
            IBindingsFactory bindingsFactory,
            IStepVerifierFactory stepVerifierFactory)
        {
            _fileSystem = fileSystem;
            _restCallerFactory = restCallerFactory;
            _blobRepositoryFactory = blobRepositoryFactory;
            _loader = loader;
            _bindingsFactory = bindingsFactory;
            _stepVerifierFactory = stepVerifierFactory;
        }

        public IScenarioRunner Create(ITestSuite testSuite)
            => new ScenarioRunner(
                _fileSystem, 
                _restCallerFactory, 
                _blobRepositoryFactory, 
                testSuite,
                _loader, 
                _bindingsFactory,
                _stepVerifierFactory);
    }
}