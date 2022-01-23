using Applique.LoadTester.Runtime.Environment;
using Applique.LoadTester.Design;
using System.Net.Http;
using System.Text;

namespace Applique.LoadTester.External
{
    internal class RequestFactory
    {
        private readonly string _basePath;
        private readonly Endpoint _endpoint;
        private readonly Bindings _bindings;
        private readonly string _args;

        public static HttpRequestMessage GetRequest(
            string basePath,
            Endpoint endpoint,
            Bindings bindings,
            object body,
            string args)
            => new RequestFactory(basePath, endpoint, bindings, args).GetRequest(body);

        private RequestFactory(string basePath, Endpoint endpoint, Bindings bindings, string args)
        {
            _basePath = basePath;
            _endpoint = endpoint;
            _bindings = bindings;
            _args = args;
        }

        private HttpRequestMessage GetRequest(object body)
        {
            var url = GetUrl();
            var content = _bindings.CreateContent(body);
            var requestMessage = new HttpRequestMessage(HttpMethod, url)
            {
                Content = content == null ? null : new StringContent(content, Encoding.UTF8, "application/json")
            };
            foreach (var header in _endpoint.Headers)
                requestMessage.Headers.Add(header.Name, _bindings.SubstituteVariables(header.Value));
            return requestMessage;
        }

        private HttpMethod HttpMethod => new(_endpoint.Method);

        private string GetUrl() => $"{GetPath()}{GetQuery()}";

        private string GetPath() => $"{_basePath}/{_bindings.SubstituteVariables(_endpoint.Path)}".Trim('/');

        private string GetQuery() => $"?{_bindings.SubstituteVariables(_args)}".TrimEnd('?');
    }
}