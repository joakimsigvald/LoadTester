using Applique.LoadTester.Business.Runtime.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Applique.LoadTester.Business.Runtime
{
    public class StepRunner
    {
        private readonly RunnableStep _step;
        private readonly Bindings _bindings;

        public static Task<TimeSpan> Run(RunnableStep step, Bindings bindings)
            => new StepRunner(step, bindings).Run();

        private StepRunner(RunnableStep step, Bindings bindings)
        {
            _step = step;
            _bindings = bindings;
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
            ValidateResponseStatus(response.StatusCode, body);
            if (_step.Blueprint.Response != null)
                HandleResponseBody(body);
        }

        private void HandleResponseBody(string body)
        {
            var pattern = _step.Blueprint.Response;
            var responseToken = _step.VerifyResponse(pattern, body);
            if (pattern is JObject pObject)
                BindObject(pObject, (JObject)responseToken);
            else if (pattern is JArray pArray)
                BindArray(pArray, (JArray)responseToken);
        }

        private void BindObject(JObject pObject, JObject val)
            => _bindings.BindVariables(pObject, val);

        private void BindArray(JArray pArray, JArray valArray)
        {
            if (pArray.Count != valArray.Count)
                throw new BindingFailed("", $"Array have different lengths: {valArray.Count}, expected {pArray.Count}");
            for (var i = 0; i < valArray.Count; i++)
                BindObject((JObject)pArray[i], (JObject)valArray[i]);
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
                if (isSuccessful
                    ? _step.Blueprint.BreakOnSuccess
                    : !_step.Blueprint.RetryOnFail)
                    break;
            }
            sw.Stop();
            return (lastResponse, sw.Elapsed);
        }

        public async Task<bool> IsSuccessful(HttpResponseMessage response)
        {
            if (!IsResponseStatusValid(response.StatusCode))
                return false;
            var body = await response.Content.ReadAsStringAsync();
            var pattern = _step.Blueprint.Response;
            if (pattern == null)
                return true;
            try
            {
                _step.VerifyResponse(pattern, body);
            }
            catch (VerificationFailed)
            {
                return false;
            }
            return true;
        }

        private void ValidateResponseStatus(HttpStatusCode actualStatus, string body)
        {
            if (!IsResponseStatusValid(actualStatus))
                throw new RunFailed($"Expected {string.Join(", ", _step.Blueprint.ExpectedStatusCodes)} but got {actualStatus}: {body}");
        }

        private bool IsResponseStatusValid(HttpStatusCode actualStatus)
            => _step.Blueprint.ExpectedStatusCodes.Contains(actualStatus);
    }
}