using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LoadTester
{
    public class Bindings : IEnumerable<Binding>
    {
        private readonly IDictionary<string, object> _variables;

        public Bindings(IDictionary<string, object> variables) => _variables = variables;

        public string SubstituteVariables(string target) => _variables.Aggregate(target, Substitute);

        public void Add(string name, object value) => _variables[name] = value;

        private string Substitute(string target, KeyValuePair<string, object> variable)
            => variable.Value is int?
            ? SubstituteInt(target, variable)
            : SubstituteValue(target, variable);

        private string SubstituteInt(string target, KeyValuePair<string, object> variable)
            => SubstituteValue(target.Replace($"\"{Embrace(variable.Key)}\"", variable.Value.ToString()), variable);

        private string SubstituteValue(string target, KeyValuePair<string, object> variable)
            => target.Replace(Embrace(variable.Key), variable.Value.ToString());

        private string Embrace(string value) => "{{" + value + "}}";

        public object Get(string name) => _variables.TryGetValue(name, out var variable) ? variable : null;

        public IEnumerator<Binding> GetEnumerator() => new BindingsEnumerator(_variables);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}