using Applique.LoadTester.Business.Runtime;
using System;
using System.Net.Http;

namespace Applique.LoadTester.Business.Design
{
    public class Service
    {
        public string Name { get; set; }
        public Uri BaseAddress => new(BaseUrl);
        public string BaseUrl { get; set; }
        public string BasePath { get; set; }
        public ApiKey ApiKey { get; set; }
        public Endpoint[] Endpoints { get; set; }
        public Header[] DefaultHeaders { get; set; } = Array.Empty<Header>();

        public HttpClient CreateClient(Bindings bindings)
        {
            var client = new HttpClient
            {
                BaseAddress = BaseAddress,
            };
            client.DefaultRequestHeaders.Add(ApiKey.Name, ApiKey.Value);
            foreach (var header in DefaultHeaders)
                client.DefaultRequestHeaders.Add(header.Name, bindings.SubstituteVariables(header.Value));
            return client;
        }
    }
}