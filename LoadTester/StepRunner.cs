using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace LoadTester
{
    public class StepRunner
    {
        private readonly RunnableStep _step;
        private readonly IDictionary<string, object> _variables;

        public static Task<TimeSpan> Run(RunnableStep step, IDictionary<string, object> variables)
            => new StepRunner(step, variables).Run();

        private StepRunner(RunnableStep step, IDictionary<string, object> variables)
        {
            _step = step;
            _variables = variables;
        }

        private async Task<TimeSpan> Run()
        {
            var (response, elapsed) = await RunRun();
            await HandleResponse(response);
            return elapsed;
        }

        private async Task HandleResponse(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new RunFailed($"{response.StatusCode}: {body}");
            if (_step.Blueprint.Response != null) {
                var pattern = _step.Blueprint.Response;
                var source = JsonConvert.DeserializeObject<JObject>(body);
                VerifyResponse(pattern, source);
                BindVariables(pattern, source);
            }
        }

        private void VerifyResponse(JObject pattern, JObject source)
        {
            var patternProperties = pattern.Properties();
            foreach (var pp in patternProperties)
            {
                var val = source.GetValue(pp.Name);
                if (pp.Value is JObject ppObject && val is JObject valObject)
                    VerifyResponse(ppObject, valObject);
                else if (TryGetValue(pp, out var expectedValue) && expectedValue != null)
                    VerifyValue(expectedValue, val.Value<string>());
            }
        }

        private void VerifyValue(string expectedValue, string actualValue)
        {
            if (expectedValue != actualValue)
                throw new VerificationFailed($"Unexpected response: {actualValue}, expected {expectedValue}");
        }

        private async Task<(HttpResponseMessage response, TimeSpan)> RunRun()
        {
            var sw = Stopwatch.StartNew();
            HttpResponseMessage lastResponse = null;
            for (int i = 0; i < _step.Blueprint.Times; i++)
            {
                await Task.Delay(_step.Blueprint.Delay);
                Console.WriteLine($"Calling {_step.Blueprint.Endpoint}, attempt {i + 1}");
                lastResponse = await _step.Run(_variables);
                var isSuccessful = await IsSuccessful(lastResponse);
                if (isSuccessful && _step.Blueprint.AbortOnSuccess
                    || !isSuccessful && _step.Blueprint.AbortOnFail)
                    break;
            }
            sw.Stop();
            return (lastResponse, sw.Elapsed);
        }

        public async Task<bool> IsSuccessful(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                return false;
            var body = await response.Content.ReadAsStringAsync();
            var pattern = _step.Blueprint.Response;
            if (pattern == null)
                return true;
            try
            {
                VerifyResponse(pattern, JsonConvert.DeserializeObject<JObject>(body));
            }
            catch (VerificationFailed)
            {
                return false;
            }
            return true;
        }

        private void BindVariables(JObject pattern, JObject source)
        {
            var patternProperties = pattern.Properties();
            foreach (var pp in patternProperties)
            {
                var val = source.GetValue(pp.Name);
                if (pp.Value is JObject ppObject && val is JObject valObject)
                    BindVariables(ppObject, valObject);
                else if (TryGetVariableName(pp, out var varName) && varName != null)
                {
                    var constant = new Constant(varName, val.Value<string>());
                    _variables[constant.Name] = constant.CreateValue();
                }
            }
        }

        private bool TryGetVariableName(JProperty p, out string varName)
        {
            varName = null;
            if (!IsString(p))
                return false;
            var val = p.Value.Value<string>();
            if (IsVariable(val))
                varName = val[2..^2];
            return true;
        }

        private bool TryGetValue(JProperty p, out string value)
        {
            value = null;
            if (!IsString(p))
                return false;
            var val = p.Value.Value<string>();
            if (!IsVariable(val))
                value = val;
            return true;
        }

        private bool IsVariable(string val)
            => val.StartsWith("{{") && val.EndsWith("}}");

        private bool IsString(JProperty p)
            => p.Value.Type == JTokenType.String;
    }
}