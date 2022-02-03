using Applique.LoadTester.Runtime.Result;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Applique.LoadTester.Domain;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Logic.Runtime.Engine;

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
            return ScenarioInstanceResult.Succeeded(this, sw.Elapsed, stepTimes);
        }
    }
}