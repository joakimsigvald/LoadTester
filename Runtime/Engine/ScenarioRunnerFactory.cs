using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain;
using Applique.LoadTester.Runtime.External;

namespace Applique.LoadTester.Runtime.Engine
{
    public class ScenarioRunnerFactory : IScenarioRunnerFactory
    {
        private readonly IFileSystem _fileSystem;
        private readonly IRestCallerFactory _restCallerFactory;
        private readonly IBlobRepositoryFactory _blobRepositoryFactory;
        private readonly IAssembler _assembler;
        private readonly IBindingsFactory _bindingsFactory;
        private readonly IStepVerifierFactory _stepVerifierFactory;

        public ScenarioRunnerFactory(
            IFileSystem fileSystem, 
            IRestCallerFactory restCallerFactory, 
            IBlobRepositoryFactory blobRepositoryFactory,
            IAssembler assembler,
            IBindingsFactory bindingsFactory,
            IStepVerifierFactory stepVerifierFactory)
        {
            _fileSystem = fileSystem;
            _restCallerFactory = restCallerFactory;
            _blobRepositoryFactory = blobRepositoryFactory;
            _assembler = assembler;
            _bindingsFactory = bindingsFactory;
            _stepVerifierFactory = stepVerifierFactory;
        }

        public IScenarioRunner Create(ITestSuite testSuite)
            => new ScenarioRunner(
                _fileSystem, 
                _restCallerFactory, 
                _blobRepositoryFactory, 
                testSuite, 
                _assembler, 
                _bindingsFactory,
                _stepVerifierFactory);
    }
}