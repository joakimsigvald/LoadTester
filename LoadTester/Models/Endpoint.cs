using System;
using System.Net.Http;

namespace LoadTester
{
    public class Endpoint
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public Header[] Headers { get; set; } = Array.Empty<Header>();
        public HttpMethod HttpMethod => new HttpMethod(Method);

        public HttpRequestMessage GetRequest(string basePath, Step step, Bindings bindings)
            => RequestFactory.GetRequest(basePath, this, step, bindings);
    }
}