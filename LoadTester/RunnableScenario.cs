﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var variables = new Dictionary<string, string>();
            foreach (var step in Steps)
            {
                for (int i = 0; i < step.Blueprint.Times; i++)
                {
                    await Task.Delay(step.Blueprint.Delay);
                    var response = await step.Run(variables);
                    var body = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                        return ScenarioInstanceResult.Failed($"{response.StatusCode}: {body}");
                    if (step.Blueprint.Response != null)
                        BindVariables(
                            variables, 
                            step.Blueprint.Response, 
                            JsonConvert.DeserializeObject<JObject>(body));
                }
            }
            sw.Stop();
            return ScenarioInstanceResult.Succeeded(sw.Elapsed);
        }

        private void BindVariables(Dictionary<string, string> variables, JObject pattern, JObject source)
        {
            var patternProperties = pattern.Properties();
            foreach (var pp in patternProperties)
            {
                var val = source.GetValue(pp.Name);
                if (TryGetVariableName(pp, out var varName))
                    variables[varName] = val.Value<string>();
                else if (pp.Value is JObject ppObject && val is JObject valObject)
                    BindVariables(variables, ppObject, valObject);
                else throw new ArgumentException("Malformed pattern");
            }
        }

        private bool TryGetVariableName(JProperty p, out string varName)
        {
            varName = null;
            if (!IsString(p))
                return false;
            var val = p.Value.Value<string>();
            if (!IsVariable(val))
                throw new ArgumentException("Invalid variable: " + val);
            varName = val.Substring(2..^2);
            return true;
        }

        private bool IsVariable(string val)
            => val.StartsWith("{{") && val.EndsWith("}}");

        private bool IsString(JProperty p)
            => p.Value.Type == JTokenType.String;
    }
}