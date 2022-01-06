using System.Collections;
using System.Collections.Generic;

namespace LoadTester
{
    public class BindingsEnumerator : IEnumerator<Binding>
    {
        private readonly IEnumerator<KeyValuePair<string, object>> _enumerator;

        public BindingsEnumerator(IDictionary<string, object> variables) => _enumerator = variables.GetEnumerator();

        public Binding Current => Map(_enumerator.Current);

        object IEnumerator.Current => Current;

        public void Dispose() => _enumerator.Dispose();

        public bool MoveNext() => _enumerator.MoveNext();

        public void Reset() => _enumerator.Reset();

        private Binding Map(KeyValuePair<string, object> kvp)
            => new Binding { Name = kvp.Key, Value = $"{kvp.Value}" };
    }
}