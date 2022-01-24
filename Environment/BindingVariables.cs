﻿using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Environment;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Applique.LoadTester.Environment.SpecialVariables;
using static Applique.LoadTester.Environment.ConstantExpressions;

namespace Applique.LoadTester.Environment
{
    internal class BindingVariables : IEnumerable<Constant>
    {
        private readonly IDictionary<string, object> _variables;

        public BindingVariables(IFileSystem fileSystem, ITestSuite suite, Constant[] constants, Model[] models)
            => _variables = CreateVariables(fileSystem, suite, constants, models);

        public IEnumerable<KeyValuePair<string, object>> Variables => _variables;

        public string SubstituteVariables(string target) => _variables.Aggregate(target, Substitute);

        public void Set(string name, object value) => _variables[name] = value;

        public void MergeWith(IEnumerable<KeyValuePair<string, object>> other)
        {
            foreach (var kvp in other)
                _variables[kvp.Key] = kvp.Value;
        }

        public IEnumerator<Constant> GetEnumerator() => new BindingsEnumerator(_variables);

        public bool TryGet(string name, out object variable) => _variables.TryGetValue(name, out variable);

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
    }
}