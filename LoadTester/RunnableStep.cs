using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LoadTester
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
                else if (TryGetValue(pp, out var expectedValue) && expectedValue != null)
                    VerifyValue(expectedValue, val.Value<string>());
            }
        }

        private bool TryGetValue(JProperty p, out string value)
        {
            value = null;
            if (!IsString(p))
                return false;
            var val = p.Value.Value<string>();
            if (!IsVariable(val))
                value = val;
            return true;
        }

        private void VerifyValue(string expectedValue, string actualValue)
        {
            if (expectedValue != actualValue)
                throw new VerificationFailed($"Unexpected response: {actualValue}, expected {expectedValue}");
        }

        public void BindVariables(JObject pattern, JObject source)
        {
            var patternProperties = pattern.Properties();
            foreach (var pp in patternProperties)
            {
                var val = source.GetValue(pp.Name);
                if (pp.Value is JObject ppObject && val is JObject valObject)
                    BindVariables(ppObject, valObject);
                else if (TryGetVariableName(pp, out var varName) && varName != null)
                {
                    var constant = new Constant(varName, val.Value<string>());
                    _bindings.Add(constant.Name, constant.CreateValue());
                }
            }
        }

        private bool TryGetVariableName(JProperty p, out string varName)
        {
            varName = null;
            if (!IsString(p))
                return false;
            var val = p.Value.Value<string>();
            if (IsVariable(val))
                varName = val[2..^2];
            return true;
        }

        private bool IsVariable(string val)
            => val.StartsWith("{{") && val.EndsWith("}}");

        private bool IsString(JProperty p)
            => p.Value.Type == JTokenType.String;
    }
}