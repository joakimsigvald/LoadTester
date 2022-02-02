using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Applique.LoadTester.Runtime.External;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;

namespace Applique.LoadTester.Runtime.Engine
{
    public class RestStepExecutor
    {
        private readonly IRestCaller _restCaller;
        private readonly RequestFactory _requestFactory;
        private readonly IBindings _bindings;
        private readonly Service _service;

        public static RestStepExecutor Create(
            IRestCallerFactory restCallerFactory,
            ITestSuite suite,
            Step step,
            IBindings bindings)
        {
            var pair = step.Endpoint.Split('.');
            var serviceName = pair[0];
            var endpointName = pair[1];
            var service = suite.Services.Single(s => s.Name == serviceName);
            var endpoint = service.Endpoints.Single(ep => ep.Name == endpointName);
            var restCaller = restCallerFactory.Create(service.BaseUrl);
            var requestFactory = new RequestFactory(service, endpoint, bindings, step);
            return new RestStepExecutor(restCaller, service, requestFactory, bindings);
        }

        private RestStepExecutor(
            IRestCaller restCaller,
            Service service,
            RequestFactory requestFactory,
            IBindings bindings)
        {
            _restCaller = restCaller;
            _service = service;
            _requestFactory = requestFactory;
            _bindings = bindings;
        }

        public async Task<HttpResponseMessage> Execute(Header[] serviceHeaders)
        {
            try
            {
                var req = _requestFactory.GetRequest(serviceHeaders);
                var res = await _restCaller.Call(req);
                return res;
            }
            catch (TaskCanceledException tex)
            {
                Console.WriteLine($"{tex.Message}");
                return null;
            }
        }

        public Header[] CreateServiceHeaders()
        {
            var serviceHeaders = _service.Headers.Select(h => new Header
            {
                Name = h.Name,
                Value = _bindings.SubstituteVariables(h.Value)
            });
            return (_service.ApiKey is null
                ? serviceHeaders
                : serviceHeaders.Prepend(new Header { Name = _service.ApiKey.Name, Value = _service.ApiKey.Value }))
                .ToArray();
        }
    }
}