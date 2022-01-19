using Applique.LoadTester.Business.Design;
using Applique.LoadTester.Business.Runtime;
using System.Net.Http;
using System.Threading.Tasks;

namespace Applique.LoadTester.External
{
    internal class RestCaller : IRestCaller
    {
        private readonly HttpClient _client;
        private readonly Service _service;
        private readonly Endpoint _endpoint;
        private readonly Bindings _bindings;

        public RestCaller(Service service, Endpoint endpoint, Bindings bindings)
        {
            _service = service;
            _endpoint = endpoint;
            _bindings = bindings;
            _client = service.CreateClient(_bindings);
        }

        public Task<HttpResponseMessage> Call(object body, string args)
        {
            var request = RequestFactory.GetRequest(_service.BasePath, _endpoint, _bindings, body, args);
            return _client.SendAsync(request);
        }
    }
}