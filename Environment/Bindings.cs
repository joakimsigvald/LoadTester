using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Environment;
using Applique.LoadTester.Runtime.Environment;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using static Applique.LoadTester.Environment.ConstantExpressions;

namespace Applique.LoadTester.Environment
{
    public class Bindings : IBindings
    {
        private readonly BindingVariables _bindingVariables;

        public Bindings(BindingVariables bindingVariables) => _bindingVariables = bindingVariables;

        public object Get(string name) => TryGet(name, out var variable) ? variable : null;

        public string SubstituteVariables(string target)
            => _bindingVariables.SubstituteVariables(target);

        public IEnumerable<KeyValuePair<string, object>> Variables => _bindingVariables.Variables;

        public void BindResponse(JToken pattern, JToken responseToken)
        {
            if (pattern is JObject pObject)
                BindObject(pObject, (JObject)responseToken);
            else if (pattern is JArray pArray)
                BindArray(pArray, (JArray)responseToken);
        }

        public void MergeWith(IBindings bindings) => _bindingVariables.MergeWith(bindings.Variables);

        public void VerifyValue(string prefix, JProperty expected, string actualValue)
        {
            if (!TrySubstituteVariable(expected.Value?.ToString(), out var expectedValue))
                CheckConstraints(prefix, GetConstraint(expected), actualValue);
            else if (!IsMatch(expectedValue, actualValue))
                throw new VerificationFailed(prefix, $"Unexpected response: {actualValue}, expected {expectedValue}");
        }

        private static bool IsMatch(object expectedValue, string actualValue)
            => expectedValue is DecimalWithTolerance decObj ? decObj.IsMatch(
                    ValueRetriever.ValueOf(new Constant { Value = actualValue, Type = "decimal" }) as decimal?)
            : $"{expectedValue}" == actualValue?.ToString();

        public IEnumerator<Constant> GetEnumerator() => _bindingVariables.GetEnumerator();

        public string CreateContent(object body)
            => body is null ? null
            : body is string s && IsVariable(s) ? CreateContent(Get(Unembrace(s)))
            : SubstituteVariables(JsonConvert.SerializeObject(body));

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

        //TODO: Varför sparas decimal som string när värden binds från responce? Testtäck
        private bool TrySubstituteVariable(string target, out object value)
        {
            value = target;
            if (!TryExtractConstant(target, out var constant))
                return true; // we successfully substituted all 0 variables in the expression (no constraints to check)
            if (constant.Overshadow)
                return false; // we didn't substitute because any stored value is disregarded and will be replaced (check constraints instead)
            if (!TryGet(constant.Name, out var val))
                return false; // we didn't substitute because no value is stored yet (check constraints instead)
            value = !IsVariable(target)
                ? ReplaceConstantExpressionWithValue(target, $"{val}")
                : constant.Tolerance != 0 
                ? new DecimalWithTolerance 
                { 
                    Value = val as decimal? ?? (decimal.TryParse($"{val}", out var dec) ? dec : null), 
                    Tolerance = constant.Tolerance
                }
                : val;
            return true; // we substitute existing variable value and will not store new value, so no need to check constraints
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void BindArray(JArray pArray, JArray valArray)
        {
            if (pArray.Count != valArray.Count)
                throw new BindingFailed("", $"Array have different lengths: {valArray.Count}, expected {pArray.Count}");
            for (var i = 0; i < valArray.Count; i++)
                BindObject((JObject)pArray[i], (JObject)valArray[i]);
        }

        private void BindObject(JObject pObject, JObject val)
            => BindVariables(pObject, val);

        private void SetValue(string varExpression, JToken val)
        {
            var constant = ConstantFactory.Create(varExpression, val.Value<string>());
            if (!constant.Overshadow && TryGet(constant.Name, out var existing))
                constant.Type = ValueRetriever.GetType(existing);
            _bindingVariables.Set(constant.Name, ValueRetriever.ValueOf(constant));
        }

        private bool TryGet(string name, out object variable) => _bindingVariables.TryGet(name, out variable);

        private void BindVariables(JProperty pp, JArray ppArray, JArray valArray)
        {
            if (ppArray.Count != valArray.Count)
                throw new BindingFailed($"{pp.Name}", $"Array have different lengths: {valArray.Count}, expected {ppArray.Count}");
            for (var i = 0; i < valArray.Count; i++)
                BindVariables((JObject)ppArray[i], (JObject)valArray[i]);
        }

        private void BindVariables(JObject pattern, JObject source)
        {
            var patternProperties = pattern.Properties();
            foreach (var pp in patternProperties)
            {
                var val = source.GetValue(pp.Name);
                if (pp.Value is JObject ppObject)
                    BindVariables(ppObject, val as JObject);
                else if (pp.Value is JArray ppArray)
                    BindVariables(pp, ppArray, val as JArray);
                else if (TryGetVariableName(pp, out var varExpression) && varExpression != null)
                    SetValue(varExpression, val);
            }
        }
    }
}