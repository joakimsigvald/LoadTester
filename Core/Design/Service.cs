using System;

namespace Applique.LoadTester.Core.Design
{
    public class Service
    {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string BasePath { get; set; }
        public ApiKey ApiKey { get; set; }
        public Endpoint[] Endpoints { get; set; }
        public Header[] Headers { get; set; } = Array.Empty<Header>();
    }
}