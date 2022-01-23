using Applique.LoadTester.Runtime.Environment;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Applique.LoadTester.Domain.Environment;
using Applique.LoadTester.Domain.Design;

namespace Applique.LoadTester.Runtime.Engine
{
    public class RestStep : RunnableStep<HttpResponseMessage>
    {
        private readonly Endpoint _endpoint;
        private readonly IBindings _bindings;
        private readonly IRestCaller _restCaller;
        private readonly StepVerifier _stepVerifier;

        public static IRunnableStep Create(
            IRestCallerFactory _restCallerFactory,
            ITestSuite suite,
            Step step,
            IBindings bindings)
        {
            var pair = step.Endpoint.Split('.');
            var serviceName = pair[0];
            var endpointName = pair[1];
            var service = suite.Services.Single(s => s.Name == serviceName);
            var endpoint = service.Endpoints.Single(ep => ep.Name == endpointName);
            var restCaller = _restCallerFactory.Create(service, endpoint, bindings);
            return new RestStep(restCaller, step, endpoint, bindings);
        }

        private RestStep(IRestCaller restCaller, Step step, Endpoint endpoint, IBindings bindings)
            : base(step)
        {
            _endpoint = endpoint;
            _bindings = bindings;
            _restCaller = restCaller;
            _stepVerifier = new StepVerifier(step, bindings);
        }

        protected override async Task HandleResponse(HttpResponseMessage response)
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

        protected override async Task<HttpResponseMessage> DoRun()
        {
            HttpResponseMessage lastResponse = null;
            for (int i = 0; i < Blueprint.Times; i++)
            {
                await Task.Delay(Delay);
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