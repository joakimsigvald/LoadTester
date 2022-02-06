using System;
using System.Linq;
using Applique.LoadTester.Runtime.External;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;
using Applique.LoadTester.Runtime.Engine;

namespace Applique.LoadTester.Logic.Runtime.Engine
{
    public class StepInstantiator
    {
        private readonly IRestCallerFactory _restCallerFactory;
        private readonly IBlobRepositoryFactory _blobFactory;
        private readonly IBindingsFactory _bindingsFactory;
        private readonly IStepVerifierFactory _stepVerifierFactory;
        private readonly ITestSuite _testSuite;
        private readonly IBindings _bindings;

        public StepInstantiator(
            IRestCallerFactory restCallerFactory,
            IBlobRepositoryFactory blobFactory,
            IBindingsFactory bindingsFactory,
            IStepVerifierFactory stepVerifierFactory,
            ITestSuite testSuite,
            IBindings bindings)
        {
            _restCallerFactory = restCallerFactory;
            _blobFactory = blobFactory;
            _bindingsFactory = bindingsFactory;
            _stepVerifierFactory = stepVerifierFactory;
            _testSuite = testSuite;
            _bindings = bindings;
        }

        public IRunnableStep Instanciate(IStep step)
            => step.Type switch
            {
                StepType.Rest => InstanciateRest(step),
                StepType.Blob => InstanciateBlob(step),
                StepType t => throw new NotImplementedException($"{t}")
            };

        private IRunnableStep InstanciateRest(IStep step)
            => new RestStep(
                step,
                _bindings,
                GetOverloads(step),
                RestStepExecutor.Create(_restCallerFactory, _testSuite, step, _bindings),
                _stepVerifierFactory.CreateVerifier(step, _bindings));

        public IRunnableStep InstanciateBlob(IStep step)
            => new BlobStep(_blobFactory, step, _testSuite.GetBlob(step.Endpoint), _bindings, GetOverloads(step));

        private IBindings GetOverloads(IStep step)
            => step.Constants.Any() ? _bindingsFactory.CreateBindings(_testSuite, step.Constants) : null;
    }
}