using System;
using System.Net.Http;

namespace Applique.LoadTester.Design
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
    }
}