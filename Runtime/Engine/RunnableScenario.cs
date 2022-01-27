using Applique.LoadTester.Runtime.Result;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Applique.LoadTester.Domain.Environment;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain;
using Applique.LoadTester.Domain.Engine;

namespace Applique.LoadTester.Runtime.Engine
{
    public class RunnableScenario
    {
        public IScenario Scenario { get; set; }
        public IRunnableStep[] Steps { get; private set; }

        private readonly IRestCallerFactory _restCallerFactory;
        private readonly IBlobRepositoryFactory _blobFactory;
        private readonly ITestSuite _testSuite;
        private readonly IBindingsFactory _bindingsFactory;
        private readonly IStepVerifierFactory _stepVerifierFactory;

        public IBindings Bindings { get; private set; }

        public RunnableScenario(
            IRestCallerFactory restCallerFactory,
            IBlobRepositoryFactory blobFactory,
            ITestSuite testSuite,
            IScenario scenario,
            IBindingsFactory bindingsFactory,
            IStepVerifierFactory stepVerifierFactory,
            int instanceId)
        {
            _restCallerFactory = restCallerFactory;
            _blobFactory = blobFactory;
            _testSuite = testSuite;
            Scenario = scenario;
            _bindingsFactory = bindingsFactory;
            _stepVerifierFactory = stepVerifierFactory;
            Bindings = _bindingsFactory.CreateInstanceBindings(testSuite, Scenario, GetModels(), instanceId);
            Steps = scenario.Steps
                .Select(testSuite.GetStepToRun)
                .Select(Instanciate)
                .ToArray();
        }

        private IRunnableStep Instanciate(Step step)
            => step.Type switch
            {
                StepType.Rest => InstanciateRest(step),
                StepType.Blob => BlobStep.Create(_blobFactory, _testSuite, step, Bindings, GetOverloads(step)),
                StepType t => throw new NotImplementedException($"{t}")
            };

        private IRunnableStep InstanciateRest(Step step)
            => RestStep.Create(
                _restCallerFactory, 
                _testSuite, 
                step,
                _stepVerifierFactory.CreateVerifier(step, Bindings),
                Bindings,
                GetOverloads(step));

        private IBindings GetOverloads(Step step)
            => step.Constants.Any() ? _bindingsFactory.CreateBindings(_testSuite, step.Constants) : null;

        public async Task<ScenarioInstanceResult> Run()
        {
            var sw = Stopwatch.StartNew();
            var stepTimes = new List<TimeSpan>();
            foreach (var step in Steps)
            {
                try
                {
                    var elapsed = await step.Run();
                    stepTimes.Add(elapsed);
                }
                catch (RunFailed sf)
                {
                    return ScenarioInstanceResult.Failed(this, $"Step: {step.Blueprint.Endpoint} failed with error {sf.Message}");
                }
            }
            sw.Stop();
            var assertResults = Scenario.Asserts.Select(Apply).ToArray();
            return assertResults.All(ar => ar.Success)
                ? ScenarioInstanceResult.Succeeded(this, sw.Elapsed, stepTimes, assertResults)
                : ScenarioInstanceResult.Failed(this, assertResults.Where(ar => !ar.Success));
        }

        private AssertResult Apply(Assert assert)
        {
            var value = Bindings.SubstituteVariables(assert.Value);
            var actualValue = Bindings.Get(assert.Name);
            var res = $"{actualValue}" == value;
            return res ? new AssertResult
            {
                Success = true,
                Message = $"{assert.Name} is {actualValue} as expected"
            }
            : new AssertResult { Message = $"{assert.Name} is {actualValue} but expected {value}" };
        }

        private Model[] GetModels() => _testSuite.Models;
    }
}