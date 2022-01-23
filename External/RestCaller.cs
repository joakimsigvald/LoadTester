using System.Net.Http;
using System.Threading.Tasks;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Environment;

namespace Applique.LoadTester.External
{
    internal class RestCaller : IRestCaller
    {
        private readonly HttpClient _client;
        private readonly Service _service;
        private readonly Endpoint _endpoint;
        private readonly IBindings _bindings;

        public RestCaller(Service service, Endpoint endpoint, IBindings bindings)
        {
            _service = service;
            _endpoint = endpoint;
            _bindings = bindings;
            _client = CreateClient(service, _bindings);
        }

        public Task<HttpResponseMessage> Call(object body, string args)
        {
            var request = RequestFactory.GetRequest(_service.BasePath, _endpoint, _bindings, body, args);
            return _client.SendAsync(request);
        }

        private static HttpClient CreateClient(Service service, IBindings bindings)
        {
            var client = new HttpClient
            {
                BaseAddress = new(service.BaseUrl),
            };
            client.DefaultRequestHeaders.Add(service.ApiKey.Name, service.ApiKey.Value);
            foreach (var header in service.DefaultHeaders)
                client.DefaultRequestHeaders.Add(header.Name, bindings.SubstituteVariables(header.Value));
            return client;
        }
    }
}