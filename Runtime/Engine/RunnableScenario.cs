using Applique.LoadTester.Runtime.Environment;
using Applique.LoadTester.Runtime.Result;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Applique.LoadTester.Domain.Environment;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Runtime.Assembly;

namespace Applique.LoadTester.Runtime.Engine
{
    public class RunnableScenario
    {
        public IScenario Scenario { get; set; }
        public IRunnableStep[] Steps { get; private set; }

        private readonly IRestCallerFactory _restCallerFactory;
        private readonly IBlobRepositoryFactory _blobFactory;
        private readonly ITestSuite _suite;
        private readonly int _instanceId;
        public Bindings Bindings { get; private set; }

        public RunnableScenario(
            IFileSystem fileSystem,
            IRestCallerFactory restCallerFactory,
            IBlobRepositoryFactory blobFactory,
            ITestSuite suite,
            IScenario scenario,
            int instanceId)
        {
            _instanceId = instanceId;
            _restCallerFactory = restCallerFactory;
            _blobFactory = blobFactory;
            _suite = suite;
            Scenario = scenario;
            Bindings = new Bindings(fileSystem, suite, GetConstants(), GetModels());
            Steps = scenario.Steps
                .Select(suite.GetStepToRun)
                .Select(Instanciate)
                .ToArray();
        }

        private IRunnableStep Instanciate(Step step)
            => step.Type switch
            {
                StepType.Rest => RestStep.Create(_restCallerFactory, _suite, step, Bindings),
                StepType.Blob => BlobStep.Create(_blobFactory, _suite, step, Bindings),
                StepType t => throw new NotImplementedException($"{t}")
            };

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
            var assertResults = Scenario.Asserts
                .Select(assert => assert.Apply(Bindings, Bindings.Get(assert.Name)))
                .ToArray();
            return assertResults.All(ar => ar.Success)
                ? ScenarioInstanceResult.Succeeded(this, sw.Elapsed, stepTimes, assertResults)
                : ScenarioInstanceResult.Failed(this, assertResults.Where(ar => !ar.Success));
        }

        private Constant[] GetConstants()
            => ConstantFactory.Merge(_suite.GetInstanceConstants(_instanceId), Scenario.Constants);

        private Model[] GetModels() => _suite.Models;
    }
}