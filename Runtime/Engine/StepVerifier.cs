using Applique.LoadTester.Runtime.Environment;
using Applique.LoadTester.Design;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Applique.LoadTester.Runtime.Engine
{
    public class StepVerifier
    {
        private readonly Bindings _bindings;
        private readonly Step _blueprint;

        public StepVerifier(Step step, Bindings bindings)
        {
            _bindings = bindings;
            _blueprint = step;
        }

        public JToken VerifyResponse(JToken pattern, string source, string prefix = "")
        {
            JToken responseToken;
            if (pattern is JObject pObject)
                VerifyModel(pObject, (JObject)(responseToken = JsonConvert.DeserializeObject<JObject>(source)), prefix);
            else if (pattern is JArray pArray)
                VerifyArray(pArray, (JArray)(responseToken = JsonConvert.DeserializeObject<JArray>(source)), prefix);
            else throw new NotImplementedException(
                $"Response is expected to be either object or array, but was {source}");
            return responseToken;
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
                    VerifyValue($"{prefix}{pp.Name}", pp, val?.ToString());
            }
        }

        private void VerifyValue(string prefix, JProperty pp, string actualValue)
        {
            if (!_bindings.TrySubstituteVariable(pp.Value?.ToString(), out var expectedValue))
                CheckConstraints(prefix, Bindings.GetConstraint(pp), actualValue);
            else if (expectedValue != actualValue)
                throw new VerificationFailed(prefix, $"Unexpected response: {actualValue}, expected {expectedValue}");
        }

        private static void CheckConstraints(string property, Constraint constraint, string actualValue)
        {
            switch (constraint)
            {
                case Constraint.Mandatory:
                    if (string.IsNullOrEmpty(actualValue))
                        throw new VerificationFailed(property, $"Constrain violated: {constraint}, value: {actualValue}");
                    break;
                default: break;
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