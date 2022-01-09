using Applique.LoadTester.Business.Design;
using Applique.LoadTester.Business.External;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Applique.LoadTester.Business.Runtime.SpecialVariables;

namespace Applique.LoadTester.Business.Runtime
{
    public class Bindings : IEnumerable<Constant>
    {
        private readonly IDictionary<string, object> _variables;

        public Bindings(IFileSystem fileSystem, TestSuite suite, params Constant[] constants) 
            => _variables = CreateVariables(fileSystem, suite, constants);

        public string SubstituteVariables(string target) => _variables.Aggregate(target, Substitute);

        public void Add(string name, object value) => _variables[name] = value;

        public void Append(Bindings bindings)
        {
            foreach (var kvp in bindings._variables)
                _variables[kvp.Key] = kvp.Value;
        }

        private static string Substitute(string target, KeyValuePair<string, object> variable)
            => variable.Value is int?
            ? SubstituteInt(target, variable)
            : SubstituteValue(target, variable);

        private static string SubstituteInt(string target, KeyValuePair<string, object> variable)
            => SubstituteValue(target.Replace($"\"{Embrace(variable.Key)}\"", variable.Value.ToString()), variable);

        private static string SubstituteValue(string target, KeyValuePair<string, object> variable)
            => target.Replace(Embrace(variable.Key), GetValue(variable.Value));

        private static string GetValue(object variableValue) =>
            (variableValue as string) switch
            {
                CurrentTime => $"{DateTime.Now}",
                _ => variableValue.ToString()
            };

        private static string Embrace(string value) => "{{" + value + "}}";

        public object Get(string name) => _variables.TryGetValue(name, out var variable) ? variable : null;

        public void BindVariables(JObject pattern, JObject source)
        {
            var patternProperties = pattern.Properties();
            foreach (var pp in patternProperties)
            {
                var val = source.GetValue(pp.Name);
                if (pp.Value is JObject ppObject && val is JObject valObject)
                    BindVariables(ppObject, valObject);
                else if (pp.TryGetVariableName(out var varName) && varName != null)
                {
                    var constant = new Constant(varName, val.Value<string>());
                    Add(constant.Name, ValueRetriever.ValueOf(constant));
                }
            }
        }

        public IEnumerator<Constant> GetEnumerator() => new BindingsEnumerator(_variables);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static IDictionary<string, object> CreateVariables(IFileSystem fileSystem, TestSuite suite, IEnumerable<Constant> constants)
        {
            var valueRetriever = new ValueRetriever(fileSystem, suite);
            return constants.ToDictionary(c => c.Name, valueRetriever.GetValue);
        }
    }
}