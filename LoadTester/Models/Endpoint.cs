using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace LoadTester
{
    public class Endpoint
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public string Method { get; set; }
        public HttpMethod HttpMethod => new HttpMethod(Method);

        public HttpRequestMessage GetRequest(string basePath, Step step, IDictionary<string, object> variables)
            => RequestFactory.GetRequest(basePath, this, step, variables);
    }
}