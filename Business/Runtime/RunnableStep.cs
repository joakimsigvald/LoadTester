using Applique.LoadTester.Business.Design;
using Applique.LoadTester.Business.Runtime.Exceptions;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Applique.LoadTester.Business.Runtime
{
    public class RunnableStep
    {
        private readonly Endpoint _endpoint;
        private readonly Bindings _bindings;
        private readonly IRestCaller _restCaller;
        private readonly StepVerifier _stepVerifier;

        public Step Blueprint { get; private set; }
        public Service Service { get; }

        public RunnableStep(IRestCaller restCaller, Step step, Service service, Endpoint endpoint, Bindings bindings)
        {
            _endpoint = endpoint;
            _bindings = bindings;
            _restCaller = restCaller;
            Blueprint = step;
            Service = service;
            _stepVerifier = new StepVerifier(step, bindings);
        }

        public async Task<TimeSpan> Run()
        {
            var sw = Stopwatch.StartNew();
            HttpResponseMessage response = await DoRun();
            sw.Stop();
            var elapsed = sw.Elapsed;
            await HandleResponse(response);
            return elapsed;
        }

        private async Task HandleResponse(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            if (!_stepVerifier.IsResponseStatusValid(response.StatusCode))
                throw new RunFailed($"Expected {string.Join(", ", Blueprint.ExpectedStatusCodes)} but got {response.StatusCode}: {body}");
            if (Blueprint.Response != null)
                HandleResponseBody(body);
        }

        private void HandleResponseBody(string body)
        {
            var pattern = Blueprint.Response;
            var responseToken = _stepVerifier.VerifyResponse(pattern, body);
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

        private async Task<HttpResponseMessage> DoRun()
        {
            HttpResponseMessage lastResponse = null;
            for (int i = 0; i < Blueprint.Times; i++)
            {
                await Task.Delay(Blueprint.Delay);
                Console.WriteLine($"Calling {Blueprint.Endpoint}, attempt {i + 1}");
                lastResponse = await _restCaller.Call(Blueprint.Body, Blueprint.Args);
                var isSuccessful = await _stepVerifier.IsSuccessful(lastResponse);
                if (isSuccessful ? Blueprint.BreakOnSuccess : !Blueprint.RetryOnFail)
                    break;
            }
            return lastResponse;
        }
    }
}