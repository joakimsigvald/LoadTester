using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using Applique.LoadTester.Domain.Service;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Assembly;

namespace Applique.LoadTester.Logic.Environment
{
    public class StepVerifier : IStepVerifier
    {
        private readonly IValueVerifier _valueVerifier;
        private readonly IStep _blueprint;
        private readonly IBindings _bindings;

        public StepVerifier(IStep step, IBindings bindings)
        {
            _blueprint = step;
            _bindings = bindings;
            _valueVerifier = new ValueVerifier(bindings);
        }

        public void VerifyResponse(JToken pattern, string source, string prefix = "")
        {
            JToken responseToken;
            if (pattern is JObject pObject)
                VerifyModel(pObject, (JObject)(responseToken = JsonConvert.DeserializeObject<JObject>(source)), prefix);
            else if (pattern is JArray pArray)
                VerifyArray(pArray, (JArray)(responseToken = JsonConvert.DeserializeObject<JArray>(source)), prefix);
            else throw new NotImplementedException(
                $"Response is expected to be either object or array, but was {source}");
            _bindings.BindResponse(pattern, responseToken);
        }

        public bool IsSuccessful(RestCallResponse response)
        {
            if (response is null || !IsResponseStatusValid(response.StatusCode))
                return false;
            var pattern = _blueprint.Response;
            if (pattern == null)
                return true;
            try
            {
                VerifyResponse(pattern, response.Body);
            }
            catch (VerificationFailed vf)
            {
                return false;
            }
            return true;
        }

        public bool IsResponseStatusValid(HttpStatusCode actualStatus)
            => _blueprint.ExpectedStatusCodes.Contains(actualStatus);

        private void VerifyModel(JObject pattern, JObject source, string prefix = "")
        {
            var patternProperties = pattern.Properties();
            foreach (var pp in patternProperties)
            {
                var actual = source.GetValue(pp.Name);
                if (pp.Value is JObject ppObject)
                    VerifyObject(pp, ppObject, actual as JObject, prefix);
                else if (pp.Value is JArray ppArray)
                    VerifyArray(ppArray, actual as JArray, $"{prefix}{pp.Name}");
                else
                    _valueVerifier.VerifyValue($"{prefix}{pp.Name}", pp, actual?.ToString());
            }
        }

        private void VerifyObject(JProperty pp, JObject ppObject, JObject valObject, string prefix)
            => VerifyModel(ppObject, valObject, $"{prefix}{pp.Name}.");

        private void VerifyArray(JArray ppArray, JArray valArray, string prefix)
        {
            if (ppArray.Count != valArray.Count)
                throw new VerificationFailed(prefix, $"Array have different lengths: {valArray.Count}, expected {ppArray.Count}");
            for (var i = 0; i < valArray.Count; i++)
                VerifyModel((JObject)ppArray[i], (JObject)valArray[i], $"{prefix}.");
        }
    }
}