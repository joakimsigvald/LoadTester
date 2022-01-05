using System.Net.Http;
using System.Threading.Tasks;

namespace LoadTester
{
    public class RunnableStep
    {
        private readonly HttpClient _client;
        private readonly Endpoint _endpoint;
        public Step Blueprint { get; private set; }
        public Service Service { get; }

        public RunnableStep(Step step, Service service, Endpoint endpoint)
        {
            _client = service.CreateClient();
            _endpoint = endpoint;
            Blueprint = step;
            Service = service;
        }

        public Task<HttpResponseMessage> Run(Bindings bindings)
        {
            var request = _endpoint.GetRequest(Service.BasePath, Blueprint, bindings);
            return _client.SendAsync(request);
        }
    }
}