using System;
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

        public RunnableScenario(TestSuite suite, Scenario scenario)
        {
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
            foreach (var step in Steps)
            {
                for (int i = 0; i < step.Step.Times; i++)
                {
                    await Task.Delay(step.Step.Delay);
                    var response = await step.Run();
                    if (response.IsSuccessStatusCode)
                        continue;
                    var body = await response.Content.ReadAsStringAsync();
                        return ScenarioInstanceResult.Failed($"{response.StatusCode}: {body}");
                }
            }
            sw.Stop();
            return ScenarioInstanceResult.Succeeded(sw.Elapsed);
        }
    }
}