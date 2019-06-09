using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LoadTester
{
    public class RunnableStep
    {
        private readonly HttpClient _client;
        private readonly Endpoint _endpoint;
        public Step Step { get; private set; }

        public RunnableStep(Step step, Service service, Endpoint endpoint)
        {
            _client = service.CreateClient();
            _endpoint = endpoint;
            Step = step;
        }

        public Task<HttpResponseMessage> Run() => _client.SendAsync(_endpoint.GetRequest(Step.Args, Step.Body));
    }
}