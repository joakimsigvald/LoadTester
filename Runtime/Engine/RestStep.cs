using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Applique.LoadTester.Domain;
using Applique.LoadTester.Runtime.External;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;

namespace Applique.LoadTester.Runtime.Engine
{
    public class RestStep : RunnableStep<HttpResponseMessage>
    {
        private readonly IRestCaller _restCaller;
        private readonly IStepVerifier _stepVerifier;
        private readonly RequestFactory _requestFactory;
        private readonly Service _service;

        public static IRunnableStep Create(
            IRestCallerFactory restCallerFactory,
            ITestSuite suite,
            Step step,
            IStepVerifier stepVerifier,
            IBindings bindings,
            IBindings overloads)
        {
            var pair = step.Endpoint.Split('.');
            var serviceName = pair[0];
            var endpointName = pair[1];
            var service = suite.Services.Single(s => s.Name == serviceName);
            var endpoint = service.Endpoints.Single(ep => ep.Name == endpointName);
            var restCaller = restCallerFactory.Create(service.BaseUrl);
            var requestFactory = new RequestFactory(service, endpoint, bindings, step);
            return new RestStep(restCaller, service, step, stepVerifier, bindings, overloads, requestFactory);
        }

        private RestStep(
            IRestCaller restCaller,
            Service service,
            Step step,
            IStepVerifier stepVerifier,
            IBindings bindings,
            IBindings overloads,
            RequestFactory requestFactory)
            : base(step, bindings, overloads)
        {
            _restCaller = restCaller;
            _service = service;
            _stepVerifier = stepVerifier;
            _requestFactory = requestFactory;
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
            var serviceHeaders = CreateServiceHeaders();
            for (int i = 0; i < Blueprint.Times; i++)
            {
                await Task.Delay(Delay);
                Console.WriteLine($"Calling {Blueprint.Endpoint}, attempt {i + 1}");
                lastResponse = await _restCaller.Call(_requestFactory.GetRequest(serviceHeaders));
                var isSuccessful = await _stepVerifier.IsSuccessful(lastResponse);
                if (isSuccessful ? Blueprint.BreakOnSuccess : !Blueprint.RetryOnFail)
                    break;
            }
            return lastResponse;
        }

        private Header[] CreateServiceHeaders()
            => _service.Headers.Select(h => new Header
            {
                Name = h.Name,
                Value = _bindings.SubstituteVariables(h.Value)
            }).Prepend(new Header { Name = _service.ApiKey.Name, Value = _service.ApiKey.Value })
            .ToArray();
    }
}