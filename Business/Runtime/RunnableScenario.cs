using Applique.LoadTester.Business.Design;
using Applique.LoadTester.Business.External;
using Applique.LoadTester.Business.Result;
using Applique.LoadTester.Business.Runtime.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Applique.LoadTester.Business.Runtime
{
    public class RunnableScenario
    {
        public Scenario Scenario { get; set; }
        public IRunnableStep[] Steps { get; private set; }

        private readonly IRestCallerFactory _restCallerFactory;
        private readonly IBlobRepositoryFactory _blobFactory;
        private readonly TestSuite _suite;
        private readonly int _instanceId;
        public Bindings Bindings { get; private set; }

        public RunnableScenario(
            IFileSystem fileSystem,
            IRestCallerFactory restCallerFactory,
            IBlobRepositoryFactory blobFactory,
            TestSuite suite,
            Scenario scenario,
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
                StepType.Blob => BlobStep.Create(_blobFactory,_suite, step, Bindings),
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
            => _suite.Constants.Concat(Scenario.Constants)
            .Prepend(new Constant("InstanceId", $"{_instanceId}"))
            .GroupBy(c => c.Name)
            .Select(Merge)
            .ToArray();

        private Model[] GetModels() => _suite.Models;

        private Constant Merge(IEnumerable<Constant> constants)
        {
            var arr = constants.ToArray();
            if (arr.Length == 0)
                return arr.Single();
            var first = arr.First();
            var last = arr.Last();
            return new Constant
            {
                Conversions = first.Conversions,
                Name = first.Name,
                Type = first.Type,
                Value = last.Value
            };
        }
    }
}