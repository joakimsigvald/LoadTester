using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Applique.LoadTester.Domain;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Logic.Runtime.Result;

namespace Applique.LoadTester.Logic.Runtime.Engine
{
    public class RunnableScenario
    {
        public IRunnableStep[] Steps { get; private set; }
        public IBindings Bindings { get; private set; }

        public RunnableScenario(IBindings bindings, IRunnableStep[] steps)
        {
            Bindings = bindings;
            Steps = steps;
        }

        public async Task<ScenarioInstanceResult> Run()
        {
            var sw = Stopwatch.StartNew();
            var stepDurations = new List<StepDuration>();
            foreach (var step in Steps)
            {
                try
                {
                    var elapsed = await step.Run();
                    stepDurations.Add(new StepDuration { Step = step.Blueprint, Duration = elapsed });
                }
                catch (RunFailed sf)
                {
                    return ScenarioInstanceResult.Failed(this, $"Step: {step.Blueprint.Endpoint} failed with error {sf.Message}");
                }
            }
            sw.Stop();
            return ScenarioInstanceResult.Succeeded(this, sw.Elapsed, stepDurations);
        }
    }
}