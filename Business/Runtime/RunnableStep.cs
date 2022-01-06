using Applique.LoadTester.Business.Design;
using Applique.LoadTester.Business.Runtime.Exceptions;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Applique.LoadTester.Business.Runtime
{
    public class RunnableStep
    {
        private readonly HttpClient _client;
        private readonly Endpoint _endpoint;
        private readonly Bindings _bindings;

        public Step Blueprint { get; private set; }
        public Service Service { get; }

        public RunnableStep(Step step, Service service, Endpoint endpoint, Bindings bindings)
        {
            _endpoint = endpoint;
            _bindings = bindings;
            _client = service.CreateClient(_bindings);
            Blueprint = step;
            Service = service;
        }

        public Task<HttpResponseMessage> Run()
        {
            var request = _endpoint.GetRequest(Service.BasePath, Blueprint, _bindings);
            return _client.SendAsync(request);
        }

        public void VerifyResponse(JObject pattern, JObject source)
        {
            var patternProperties = pattern.Properties();
            foreach (var pp in patternProperties)
            {
                var val = source.GetValue(pp.Name);
                if (pp.Value is JObject ppObject && val is JObject valObject)
                    VerifyResponse(ppObject, valObject);
                else if (pp.TryGetValue(out var expectedValue) && expectedValue != null)
                    VerifyValue(expectedValue, val.Value<string>());
            }
        }

        private static void VerifyValue(string expectedValue, string actualValue)
        {
            if (expectedValue != actualValue)
                throw new VerificationFailed($"Unexpected response: {actualValue}, expected {expectedValue}");
        }
    }
}