using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Runtime.Environment;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using static Applique.LoadTester.Environment.ConstantExpressions;
using static Applique.LoadTester.Environment.ConstantFactory;

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

        public string CreateContent(object body)
            => body is null ? null
            : body is string s && IsConstant(s) ? CreateContent(Get(Unembrace(s)))
            : SubstituteVariables(JsonConvert.SerializeObject(body));

        public IEnumerator<Constant> GetEnumerator() => _bindingVariables.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void SetValue(Constant constant, JToken val)
        {
            constant.Value = val.Value<string>();
            if (!constant.Overshadow && _bindingVariables.TryGet(constant.Name, out var existing))
                constant.Type = TypeOf(existing);
            _bindingVariables.Set(constant.Name, ValueOf(constant));
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