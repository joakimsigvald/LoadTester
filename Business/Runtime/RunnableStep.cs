﻿using Applique.LoadTester.Business.Design;
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
            var request = RequestFactory.GetRequest(Service.BasePath, _endpoint, Blueprint, _bindings);
            return _client.SendAsync(request);
        }

        public void VerifyResponse(JObject pattern, JObject source, string prefix = "")
        {
            var patternProperties = pattern.Properties();
            foreach (var pp in patternProperties)
            {
                var val = source.GetValue(pp.Name);
                if (pp.Value is JObject ppObject)
                    VerifyObject(pp, ppObject, val as JObject, prefix);
                else if (pp.Value is JArray ppArray)
                    VerifyArray(pp, ppArray, val as JArray, prefix);
                else if (_bindings.TryGetValue(pp, out var expectedValue))
                    VerifyValue($"{prefix}{pp.Name}", expectedValue, val.Value<string>());
            }
        }

        private void VerifyObject(JProperty pp, JObject ppObject, JObject valObject, string prefix)
            => VerifyResponse(ppObject, valObject, $"{prefix}{pp.Name}.");

        private void VerifyArray(JProperty pp, JArray ppArray, JArray valArray, string prefix)
        {
            if (ppArray.Count != valArray.Count)
                throw new VerificationFailed($"{prefix}{pp.Name}", $"Array have different lengths: {valArray.Count}, expected {ppArray.Count}");
            for (var i = 0; i < valArray.Count; i++)
                VerifyResponse((JObject)ppArray[i], (JObject)valArray[i], $"{prefix}{pp.Name}.");
        }

        private static void VerifyValue(string property, string expectedValue, string actualValue)
        {
            if (expectedValue != actualValue)
                throw new VerificationFailed(property, $"Unexpected response: {actualValue}, expected {expectedValue}");
        }
    }
}