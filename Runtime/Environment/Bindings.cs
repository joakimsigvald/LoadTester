using Applique.LoadTester.Domain;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Environment;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Applique.LoadTester.Runtime.Environment.SpecialVariables;

namespace Applique.LoadTester.Runtime.Environment
{
    public enum Constraint { None, Mandatory }

    public class Bindings : IBindings
    {
        private readonly IDictionary<string, object> _variables;

        public Bindings(IFileSystem fileSystem, ITestSuite suite, Constant[] constants, Model[] models)
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

        public bool TrySubstituteVariable(string target, out string value)
        {
            value = target;
            if (!TryExtractConstant(value, out var constant))
                return true; // we successfully substituted all 0 variables in the expression (no constraints to check)
            if (constant.Overshadow)
                return false; // we didn't substitute because any stored value is disregarded and will be replaced (check constraints instead)
            if (!TryGet(constant.Name, out var val))
                return false; // we didn't substitute because no value is stored yet (check constraints instead)
            value = ReplaceConstantExpressionWithValue(value, $"{val}");
            return true; // we substitute existing variable value and will not store new value, so no need to check constraints
        }

        private static string ReplaceConstantExpressionWithValue(string target, string value)
        {
            var startIndex = target.IndexOf("{{");
            var endIndex = target.IndexOf("}}");
            return target[..startIndex] + value + target[(endIndex + 2)..];
        }

        public object Get(string name) => TryGet(name, out var variable) ? variable : null;

        public IEnumerator<Constant> GetEnumerator() => new BindingsEnumerator(_variables);

        public string CreateContent(object body)
            => body is null ? null
            : body is string s && IsVariable(s) ? CreateContent(Get(Unembrace(s)))
            : SubstituteVariables(JsonConvert.SerializeObject(body));

        private static Constraint ParseConstraint(string str)
            => Enum.TryParse<Constraint>(str, out var val) ? val : default;

        private void SetValue(string varExpression, JToken val)
        {
            var constant = ConstantFactory.Create(varExpression, val.Value<string>());
            if (!constant.Overshadow && TryGet(constant.Name, out var existing))
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
            => ValueRetriever.IsBool(variable.Value)
            ? SubstituteBool(target, variable)
            : ValueRetriever.IsScalar(variable.Value)
            ? SubstituteScalar(target, variable)
            : SubstituteValue(target, variable);

        private static string SubstituteBool(string target, KeyValuePair<string, object> variable)
            => SubstituteScalar(target, variable.Key, $"{variable.Value}".ToLower());

        private static string SubstituteScalar(string target, KeyValuePair<string, object> variable)
            => SubstituteScalar(target, variable.Key, $"{variable.Value}");

        private static string SubstituteScalar(string target, string prop, string value)
        {
            var constantName = Embrace(prop);
            var from1 = $"\"{constantName}\"";// replace string with value in json model
            var from2 = $"'{constantName}'"; // when we want to embedd a scalar in a string, the variable has to be surrounded by single quotes
            var intermediate1 = target.Replace(from1, value);
            var intermediate2 = intermediate1.Replace(from2, value);
            var res = intermediate2.Replace(constantName, value);
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
            ITestSuite suite,
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
                varName = ConstantFactory.Create(Unembrace(val)).Name;
            return true;
        }

        private static string Embrace(string value) => "{{" + value + "}}";

        private static string Unembrace(string variable) => variable[2..^2];

        private static bool IsVariable(string val)
            => val?.StartsWith("{{") == true && val?.EndsWith("}}") == true;

        private static bool TryExtractConstant(string val, out Constant constant)
        {
            constant = default;
            var startIndex = val?.IndexOf("{{") ?? -1;
            var endIndex = val?.IndexOf("}}") ?? -1;
            if (startIndex < 0 || startIndex >= endIndex)
                return false;
            var str = val[(startIndex + 2)..endIndex];
            constant = ConstantFactory.Create(str);
            return true;
        }

        private static bool IsString(JProperty p)
            => p.Value.Type == JTokenType.String;

    }
}