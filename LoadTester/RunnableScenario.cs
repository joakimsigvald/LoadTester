using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LoadTester
{
    public class RunnableScenario
    {
        public Scenario Scenario { get; set; }
        public RunnableStep[] Steps { get; private set; }
        private readonly TestSuite _suite;
        private readonly int _instanceId;

        public RunnableScenario(TestSuite suite, Scenario scenario, int instanceId)
        {
            _instanceId = instanceId;
            _suite = suite;
            Scenario = scenario;
            Steps = scenario.Steps.Select(Instanciate).ToArray();
        }

        private RunnableStep Instanciate(Step step)
        {
            var pair = step.Endpoint.Split('.');
            var serviceName = pair[0];
            var endpointName = pair[1];
            var service = _suite.Services.Single(s => s.Name == serviceName);
            var endpoint = service.Endpoints.Single(ep => ep.Name == endpointName);
            return new RunnableStep(step, service, endpoint);
        }

        public async Task<ScenarioInstanceResult> Run()
        {
            var sw = Stopwatch.StartNew();
            var variables = CreateVariables();
            var stepTimes = new List<TimeSpan>();
            foreach (var step in Steps)
            {
                try
                {
                    var elapsed = await StepRunner.Run(step, variables);
                    stepTimes.Add(elapsed);
                }
                catch (ScenarioFailed sf)
                {
                    return ScenarioInstanceResult.Failed(sf.Message);
                }
            }
            sw.Stop();
            return ScenarioInstanceResult.Succeeded(sw.Elapsed, stepTimes);
        }

        private IDictionary<string, object> CreateVariables()
            => _suite.Constants
                .Prepend(new Constant("InstanceId", $"{_instanceId}"))
                .ToDictionary(c => c.Name, c => c.CreateValue());
    }
}