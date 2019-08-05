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
        public Service Service { get; }

        public RunnableStep(Step step, Service service, Endpoint endpoint)
        {
            _client = service.CreateClient();
            _endpoint = endpoint;
            Blueprint = step;
            Service = service;
        }

        public Task<HttpResponseMessage> Run(IDictionary<string, object> variables) 
            => _client.SendAsync(_endpoint.GetRequest(Service.BasePath, Blueprint, variables));
    }
}