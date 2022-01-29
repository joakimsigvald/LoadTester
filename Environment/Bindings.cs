using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Runtime.Environment;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Applique.LoadTester.Environment.ConstantExpressions;

namespace Applique.LoadTester.Environment
{
    public class Bindings : IBindings
    {
        private readonly BindingVariables _bindingVariables;
        private IBindings _overloads;

        public Bindings(BindingVariables bindingVariables) => _bindingVariables = bindingVariables;

        public object Get(string name) => TryGet(name, out var val) ? val : null;

        public bool TryGet(string name, out object val)
        {
            val = null;
            return _overloads?.TryGet(name, out val) == true || _bindingVariables.TryGet(name, out val);
        }

        public string SubstituteVariables(string target)
            => _bindingVariables.SubstituteVariables(_overloads?.SubstituteVariables(target) ?? target);

        public IEnumerable<KeyValuePair<string, object>> Variables => _bindingVariables.Variables;

        public void BindResponse(JToken pattern, JToken responseToken)
        {
            if (pattern is JObject pObject)
                BindVariables(pObject, (JObject)responseToken);
            else if (pattern is JArray pArray)
                BindArray(pArray, (JArray)responseToken);
        }

        public void MergeWith(IBindings bindings) => _bindingVariables.MergeWith(bindings.Variables);

        public void OverloadWith(IBindings bindings) => _overloads = bindings;

        public void VerifyValue(string prefix, JProperty expected, string actualValue)
        {
            if (!TrySubstituteVariable(expected.Value?.ToString(), out var expectedValue))
                CheckConstraints(prefix, GetConstraint(expected), actualValue);
            else if (!IsMatch(expectedValue, actualValue))
                throw new VerificationFailed(prefix, $"Unexpected response: {actualValue}, expected {expectedValue}");
        }

        public IEnumerator<Constant> GetEnumerator() => _bindingVariables.GetEnumerator();

        public string CreateContent(object body)
            => body is null ? null
            : body is string s && IsVariable(s) ? CreateContent(Get(Unembrace(s)))
            : SubstituteVariables(JsonConvert.SerializeObject(body));

        private static bool IsMatch(object expectedValue, string actualValue)
            => expectedValue is DecimalWithTolerance decObj ? decObj.IsMatch(
                    ValueRetriever.ValueOf(new Constant { Value = actualValue, Type = "decimal" }) as decimal?)
            : $"{expectedValue}" == actualValue?.ToString();

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

        private bool TryExtractFormula(string target, out object[] terms)
        {
            terms = null;
            if (target?.StartsWith('=') != true)
                return false;
            terms = target[1..].Split('+').Select(Unembrace).Select(Get).ToArray();
            return terms.Length > 1 && terms.All(t => t is decimal);
        }

        private bool TrySubstituteVariable(string target, out object value)
        {
            value = target;
            if (TryExtractFormula(target, out var expr))
            {
                value = expr.Cast<decimal>().Aggregate(0M, (a, b) => a + b);
                return true;
            }
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
                    Value = val as decimal?,
                    Tolerance = constant.Tolerance
                }
                : val;
            return true; // we substitute existing variable value and will not store new value, so no need to check constraints
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void SetValue(Constant constant, JToken val)
        {
            constant.Value = val.Value<string>();
            if (!constant.Overshadow && _bindingVariables.TryGet(constant.Name, out var existing))
                constant.Type = ValueRetriever.GetType(existing);
            _bindingVariables.Set(constant.Name, ValueRetriever.ValueOf(constant));
        }

        private void BindVariables(JObject pattern, JObject source, string prefix = null)
        {
            var patternProperties = pattern.Properties();
            foreach (var pp in patternProperties)
                BindVariable($"{prefix}.{pp.Name}".TrimStart('.'), pp);

            void BindVariable(string prefix, JProperty pp)
            {
                var val = source.GetValue(pp.Name);
                if (pp.Value is JObject ppObject)
                    BindVariables(ppObject, val as JObject, prefix);
                else if (pp.Value is JArray ppArray)
                    BindArray(ppArray, val as JArray, prefix);
                else if (TryGetConstant(pp.Value.Value<string>(), out var constant))
                    SetValue(constant, val);
            }
        }

        private void BindArray(JArray pArray, JArray valArray, string prefix = null)
        {
            if (pArray.Count != valArray.Count)
                throw new BindingFailed(prefix, $"Array have different lengths: {valArray.Count}, expected {pArray.Count}");
            for (var i = 0; i < valArray.Count; i++)
                BindVariables((JObject)pArray[i], (JObject)valArray[i], $"{prefix}[{i}]");
        }
    }
}