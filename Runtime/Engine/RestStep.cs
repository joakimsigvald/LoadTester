using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Applique.LoadTester.Domain.Environment;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Engine;

namespace Applique.LoadTester.Runtime.Engine
{
    public class RestStep : RunnableStep<HttpResponseMessage>
    {
        private readonly Endpoint _endpoint;
        private readonly IRestCaller _restCaller;
        private readonly IStepVerifier _stepVerifier;

        public static IRunnableStep Create(
            IRestCallerFactory restCallerFactory,
            ITestSuite suite,
            Step step,
            IBindings bindings,
            IStepVerifier stepVerifier)
        {
            var pair = step.Endpoint.Split('.');
            var serviceName = pair[0];
            var endpointName = pair[1];
            var service = suite.Services.Single(s => s.Name == serviceName);
            var endpoint = service.Endpoints.Single(ep => ep.Name == endpointName);
            var restCaller = restCallerFactory.Create(service, endpoint, bindings);
            return new RestStep(restCaller, step, endpoint, stepVerifier);
        }

        private RestStep(IRestCaller restCaller, Step step, Endpoint endpoint, IStepVerifier stepVerifier)
            : base(step)
        {
            _endpoint = endpoint;
            _restCaller = restCaller;
            _stepVerifier = stepVerifier;
        }

        protected override async Task HandleResponse(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            if (!_stepVerifier.IsResponseStatusValid(response.StatusCode))
                throw new RunFailed($"Expected {string.Join(", ", Blueprint.ExpectedStatusCodes)} but got {response.StatusCode}: {body}");
            if (Blueprint.Response != null)
                _stepVerifier.VerifyResponse(Blueprint.Response, body);
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