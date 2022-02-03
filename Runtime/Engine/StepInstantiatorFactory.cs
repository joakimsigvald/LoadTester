using Applique.LoadTester.Runtime.External;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;
using Applique.LoadTester.Logic.Runtime.Engine;

namespace Applique.LoadTester.Runtime.Engine
{
    public class StepInstantiatorFactory
    {
        private readonly IRestCallerFactory _restCallerFactory;
        private readonly IBlobRepositoryFactory _blobRepositoryFactory;
        private readonly IBindingsFactory _bindingsFactory;
        private readonly IStepVerifierFactory _stepVerifierFactory;

        public StepInstantiatorFactory(
            IRestCallerFactory restCallerFactory,
            IBlobRepositoryFactory blobRepositoryFactory,
            IBindingsFactory bindingsFactory,
            IStepVerifierFactory stepVerifierFactory)
        {
            _restCallerFactory = restCallerFactory;
            _blobRepositoryFactory = blobRepositoryFactory;
            _bindingsFactory = bindingsFactory;
            _stepVerifierFactory = stepVerifierFactory;
        }

        public StepInstantiator Create(ITestSuite testSuite, IBindings bindings)
            => new(_restCallerFactory,
                _blobRepositoryFactory,
                _bindingsFactory,
                _stepVerifierFactory,
                testSuite,
                bindings);
    }
}