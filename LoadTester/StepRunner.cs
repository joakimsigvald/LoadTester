﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace LoadTester
{
    public class StepRunner
    {
        private readonly RunnableStep _step;

        public static Task<TimeSpan> Run(RunnableStep step)
            => new StepRunner(step).Run();

        private StepRunner(RunnableStep step) => _step = step;

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
                _step.VerifyResponse(pattern, source);
                _step.BindVariables(pattern, source);
            }
        }

        private async Task<(HttpResponseMessage response, TimeSpan)> RunRun()
        {
            var sw = Stopwatch.StartNew();
            HttpResponseMessage lastResponse = null;
            for (int i = 0; i < _step.Blueprint.Times; i++)
            {
                await Task.Delay(_step.Blueprint.Delay);
                Console.WriteLine($"Calling {_step.Blueprint.Endpoint}, attempt {i + 1}");
                lastResponse = await _step.Run();
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
                _step.VerifyResponse(pattern, JsonConvert.DeserializeObject<JObject>(body));
            }
            catch (VerificationFailed)
            {
                return false;
            }
            return true;
        }
    }
}