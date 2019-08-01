using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace LoadTester
{
    public class Endpoint
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public string Method { get; set; }
        public HttpMethod HttpMethod => new HttpMethod(Method);

        public HttpRequestMessage GetRequest(string basePath, Step step, Dictionary<string, string> variables)
        {
            var url = GetUrl(basePath, step, variables);
            var content = CreateContent(step.Body);
            return new HttpRequestMessage(HttpMethod, url)
            {
                Content = content
            };
        }

        private HttpContent CreateContent(dynamic body)
            => body is null
            ? null
            : new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

        private string GetUrl(string basePath, Step step, Dictionary<string, string> variables)
            => $"{basePath}/{GetPath(step, variables)}?{GetQuery(step, variables)}".TrimStart('/').TrimEnd('?');

        private string GetPath(Step step, Dictionary<string, string> variables)
            => step.Path is null
            ? Path
            : $"{Path}/{SubstituteVariables(step.Path, variables)}";

        private string GetQuery(Step step, Dictionary<string, string> variables)
            => $"?{SubstituteVariables(step.Args, variables)}";

        private string SubstituteVariables(string target, Dictionary<string, string> variables)
        {
            foreach (var kvp in variables)
                target = target.Replace("{{" + kvp.Key + "}}", kvp.Value.ToString());
            return target;
        }
    }
}