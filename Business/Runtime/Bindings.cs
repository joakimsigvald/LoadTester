using Applique.LoadTester.Business.Design;
using Applique.LoadTester.Business.External;
using Applique.LoadTester.Business.Runtime.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Applique.LoadTester.Business.Runtime.SpecialVariables;

namespace Applique.LoadTester.Business.Runtime
{
    public enum Constraint { None, Mandatory }

    public class Bindings : IEnumerable<Constant>
    {
        private readonly IDictionary<string, object> _variables;

        public Bindings(IFileSystem fileSystem, TestSuite suite, Constant[] constants, Model[] models)
            => _variables = CreateVariables(fileSystem, suite, constants, models);

        public string SubstituteVariables(string target) => _variables.Aggregate(target, Substitute);

        public void BindVariables(JObject pattern, JObject source)
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

        public static Constraint GetConstraint(JProperty pp)
        {
            if (!IsString(pp))
                return default;
            var val = pp.Value.Value<string>();
            if (!IsVariable(val))
                return default;
            var expr = Unembrace(val);
            var parts = expr.Split(' ');
            if (parts.Length != 2)
                return default;
            return ParseConstraint(parts[1]);
        }

        public void Set(string name, object value) => _variables[name] = value;

        public void MergeWith(Bindings bindings)
        {
            foreach (var kvp in bindings._variables)
                _variables[kvp.Key] = kvp.Value;
        }

        public bool TryGetValue(JProperty p, out string value)
        {
            value = p.Value?.ToString();
            if (!IsVariable(value))
                return true;
            var constant = new Constant(Unembrace(value), null);
            if (constant.Overshadow)
                return false;
            var hasValue = TryGet(constant.Name, out var val);
            value = val?.ToString();
            return hasValue;
        }

        public object Get(string name) => TryGet(name, out var variable) ? variable : null;

        public IEnumerator<Constant> GetEnumerator() => new BindingsEnumerator(_variables);

        public string CreateContent(object body)
            => body is null
            ? null
            : body is string s && IsVariable(s) ? CreateContent(Get(Unembrace(s)))
            : SubstituteVariables(JsonConvert.SerializeObject(body));

        private static Constraint ParseConstraint(string str)
            => Enum.TryParse<Constraint>(str, out var val) ? val : default;

        private void SetValue(string varExpression, JToken val)
        {
            var constant = new Constant(varExpression, val.Value<string>());
            if (!varExpression.StartsWith(':') // Replace existing constant, including type
                && TryGet(constant.Name, out var existing))
                constant.Type = ValueRetriever.GetType(existing);
            Set(constant.Name, ValueRetriever.ValueOf(constant));
        }

        private bool TryGet(string name, out object variable) => _variables.TryGetValue(name, out variable);

        private void BindVariables(JProperty pp, JArray ppArray, JArray valArray)
        {
            if (ppArray.Count != valArray.Count)
                throw new BindingFailed($"{pp.Name}", $"Array have different lengths: {valArray.Count}, expected {ppArray.Count}");
            for (var i = 0; i < valArray.Count; i++)
                BindVariables((JObject)ppArray[i], (JObject)valArray[i]);
        }

        private static string Substitute(string target, KeyValuePair<string, object> variable)
            => variable.Value is int?
            ? SubstituteInt(target, variable)
            : SubstituteValue(target, variable);

        private static string SubstituteInt(string target, KeyValuePair<string, object> variable)
        {
            var from = $"\"{Embrace(variable.Key)}\"";
            var to = $"{variable.Value}";
            var res = SubstituteValue(target.Replace(from, to), variable);
            return res;
        }

        private static string SubstituteValue(string target, KeyValuePair<string, object> variable)
        {
            var from = Embrace(variable.Key);
            var to = GetValue(variable.Value);
            var res = target.Replace(from, to);
            return res;
        }

        private static string GetValue(object variableValue) =>
            (variableValue as string) switch
            {
                CurrentTime => $"{DateTime.Now}",
                _ => $"{variableValue}"
            };

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static IDictionary<string, object> CreateVariables(
            IFileSystem fileSystem,
            TestSuite suite,
            IEnumerable<Constant> constants,
            Model[] models)
        {
            var valueRetriever = new ValueRetriever(fileSystem, suite);
            var variables = constants.ToDictionary(c => c.Name, valueRetriever.GetValue);
            foreach (var model in models)
                variables[model.Name] = model.Value;
            return variables;
        }

        private static bool TryGetVariableName(JProperty p, out string varName)
        {
            varName = null;
            if (!IsString(p))
                return false;
            var val = p.Value.Value<string>();
            if (IsVariable(val))
                varName = new Constant(Unembrace(val), null).Name;
            return true;
        }

        private static string Embrace(string value) => "{{" + value + "}}";

        private static string Unembrace(string variable) => variable[2..^2];

        private static bool IsVariable(string val)
            => val?.StartsWith("{{") == true && val?.EndsWith("}}") == true;

        private static bool IsString(JProperty p)
            => p.Value.Type == JTokenType.String;

    }
}