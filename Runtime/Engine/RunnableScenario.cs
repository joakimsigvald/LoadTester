using Applique.LoadTester.Runtime.Result;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Applique.LoadTester.Domain;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;

namespace Applique.LoadTester.Runtime.Engine
{
    public class RunnableScenario
    {
        public IScenario Scenario { get; set; }
        public IRunnableStep[] Steps { get; private set; }

        private readonly StepInstantiator _stepInstantiator;

        public IBindings Bindings { get; private set; }

        public RunnableScenario(
            ITestSuite testSuite, 
            IScenario scenario, 
            IBindings bindings, 
            StepInstantiator stepInstantiator)
        {
            Scenario = scenario;
            _stepInstantiator = stepInstantiator;
            Bindings = bindings;
            Steps = scenario.Steps
                .Select(testSuite.GetStepToRun)
                .Select(_stepInstantiator.Instanciate)
                .ToArray();
        }

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
    }
}