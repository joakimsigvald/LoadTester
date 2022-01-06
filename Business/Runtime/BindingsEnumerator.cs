﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Applique.LoadTester.Business.Runtime
{
    public class BindingsEnumerator : IEnumerator<Binding>
    {
        private readonly IEnumerator<KeyValuePair<string, object>> _enumerator;

        public BindingsEnumerator(IDictionary<string, object> variables) => _enumerator = variables.GetEnumerator();

        public Binding Current => Map(_enumerator.Current);

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            _enumerator.Dispose();
            GC.SuppressFinalize(this);
        }

        public bool MoveNext() => _enumerator.MoveNext();

        public void Reset() => _enumerator.Reset();

        private static Binding Map(KeyValuePair<string, object> kvp)
            => new() { Name = kvp.Key, Value = $"{kvp.Value}" };
    }
}