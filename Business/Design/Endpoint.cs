using Applique.LoadTester.Business.Runtime;
using System;
using System.Net.Http;

namespace Applique.LoadTester.Business.Design
{
    public class Endpoint
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public Header[] Headers { get; set; } = Array.Empty<Header>();
        public HttpMethod HttpMethod => new(Method);

        public HttpRequestMessage GetRequest(string basePath, Step step, Bindings bindings)
            => RequestFactory.GetRequest(basePath, this, step, bindings);
    }
}