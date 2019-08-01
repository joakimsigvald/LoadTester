using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace LoadTester
{
    public class RequestFactory
    {
        private readonly string _basePath;
        private readonly Endpoint _endpoint;
        private readonly Dictionary<string, string> _variables;
        private readonly Step _step;

        public static HttpRequestMessage GetRequest(string basePath, Endpoint endpoint, Step step, Dictionary<string, string> variables)
            => new RequestFactory(basePath, endpoint, step, variables).GetRequest();

        private RequestFactory(string basePath, Endpoint endpoint, Step step, Dictionary<string, string> variables)
        {
            _basePath = basePath;
            _endpoint = endpoint;
            _step = step;
            _variables = variables;
        }

        private HttpRequestMessage GetRequest()
        {
            var url = GetUrl();
            var content = CreateContent();
            return new HttpRequestMessage(_endpoint.HttpMethod, url)
            {
                Content = content == null ? null : new StringContent(content, Encoding.UTF8, "application/json")
            };
        }

        private string CreateContent()
            => _step.Body is null
            ? null
            : SubstituteVariables(JsonConvert.SerializeObject(_step.Body));

        private string GetUrl() => $"{GetPath()}{GetQuery()}";

        private string GetPath() => $"{_basePath}/{SubstituteVariables(_endpoint.Path)}".Trim('/');

        private string GetQuery() => $"?{SubstituteVariables(_step.Args)}".TrimEnd('?');

        private string SubstituteVariables(string target) => _variables.Aggregate(target, Substitute);

        private string Substitute(string target, KeyValuePair<string, string> kvp)
            => target.Replace("{{" + kvp.Key + "}}", kvp.Value.ToString())
            .Replace("\"{{" + kvp.Key + ":int}}\"", kvp.Value.ToString());
    }
}