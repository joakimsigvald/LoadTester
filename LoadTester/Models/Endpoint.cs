using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace LoadTester
{
    public class Endpoint
    {
        public string Name { get; set; }
        public string Service { get; set; }
        public string Path { get; set; }

        public string Method { get; set; }
        public HttpMethod HttpMethod => new HttpMethod(Method);

        public HttpRequestMessage GetRequest(string args, dynamic body) 
            => new HttpRequestMessage(HttpMethod, GetUrl(args)) {
                Content = CreateContent(body)
            };

        private HttpContent CreateContent(dynamic body)
            => body is null
            ? null
            : new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

        private string GetUrl(string args) => $"{Path}?{args}";
    }
}