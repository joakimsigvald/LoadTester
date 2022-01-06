using Applique.LoadTester.Business.Design;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Applique.LoadTester.Business.Runtime
{
    public class RequestFactory
    {
        private readonly string _basePath;
        private readonly Endpoint _endpoint;
        private readonly Bindings _bindings;
        private readonly Step _step;

        public static HttpRequestMessage GetRequest(string basePath, Endpoint endpoint, Step step, Bindings bindings)
            => new RequestFactory(basePath, endpoint, step, bindings).GetRequest();

        private RequestFactory(string basePath, Endpoint endpoint, Step step, Bindings bindings)
        {
            _basePath = basePath;
            _endpoint = endpoint;
            _step = step;
            _bindings = bindings;
        }

        private HttpRequestMessage GetRequest()
        {
            var url = GetUrl();
            var content = CreateContent();
            var requestMessage = new HttpRequestMessage(_endpoint.HttpMethod, url)
            {
                Content = content == null ? null : new StringContent(content, Encoding.UTF8, "application/json")
            };
            foreach (var header in _endpoint.Headers)
                requestMessage.Headers.Add(header.Name, _bindings.SubstituteVariables(header.Value));
            return requestMessage;
        }

        private string CreateContent()
            => _step.Body is null
            ? null
            : _bindings.SubstituteVariables(JsonConvert.SerializeObject(_step.Body));

        private string GetUrl() => $"{GetPath()}{GetQuery()}";

        private string GetPath() => $"{_basePath}/{_bindings.SubstituteVariables(_endpoint.Path)}".Trim('/');

        private string GetQuery() => $"?{_bindings.SubstituteVariables(_step.Args)}".TrimEnd('?');
    }
}