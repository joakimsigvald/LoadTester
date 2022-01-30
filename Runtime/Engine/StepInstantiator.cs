using System;
using System.Linq;
using Applique.LoadTester.Runtime.External;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;

namespace Applique.LoadTester.Runtime.Engine
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

        public IRunnableStep Instanciate(Step step)
            => step.Type switch
            {
                StepType.Rest => InstanciateRest(step),
                StepType.Blob => InstanciateBlob(step),
                StepType t => throw new NotImplementedException($"{t}")
            };

        private IRunnableStep InstanciateRest(Step step)
            => RestStep.Create(
                _restCallerFactory,
                _testSuite,
                step,
                _stepVerifierFactory.CreateVerifier(step, _bindings),
                _bindings,
                GetOverloads(step));

        public IRunnableStep InstanciateBlob(Step step)
            => BlobStep.Create(_blobFactory, _testSuite, step, _bindings, GetOverloads(step));

        private IBindings GetOverloads(Step step)
            => step.Constants.Any() ? _bindingsFactory.CreateBindings(_testSuite, step.Constants) : null;
    }
}