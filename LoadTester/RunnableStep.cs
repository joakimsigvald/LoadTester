using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LoadTester
{
    public class RunnableStep
    {
        private readonly HttpClient _client;
        private readonly Endpoint _endpoint;
        public Step Blueprint { get; private set; }

        public RunnableStep(Step step, Service service, Endpoint endpoint)
        {
            _client = service.CreateClient();
            _endpoint = endpoint;
            Blueprint = step;
        }

        public Task<HttpResponseMessage> Run(Dictionary<string, string> variables) 
            => _client.SendAsync(_endpoint.GetRequest(Blueprint, variables));
    }
}