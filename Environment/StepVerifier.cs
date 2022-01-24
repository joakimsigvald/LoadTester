using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Environment;

namespace Applique.LoadTester.Environment
{
    public class StepVerifier : IStepVerifier
    {
        private readonly IBindings _bindings;
        private readonly Step _blueprint;

        public StepVerifier(Step step, IBindings bindings)
        {
            _bindings = bindings;
            _blueprint = step;
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

        public async Task<bool> IsSuccessful(HttpResponseMessage response)
        {
            if (!IsResponseStatusValid(response.StatusCode))
                return false;
            var body = await response.Content.ReadAsStringAsync();
            var pattern = _blueprint.Response;
            if (pattern == null)
                return true;
            try
            {
                VerifyResponse(pattern, body);
            }
            catch (VerificationFailed)
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
                var val = source.GetValue(pp.Name);
                if (pp.Value is JObject ppObject)
                    VerifyObject(pp, ppObject, val as JObject, prefix);
                else if (pp.Value is JArray ppArray)
                    VerifyArray(ppArray, val as JArray, $"{prefix}{pp.Name}");
                else
                    _bindings.VerifyValue($"{prefix}{pp.Name}", pp, val?.ToString());
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