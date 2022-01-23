using System;
using System.Collections.Generic;
using System.Linq;

namespace Applique.LoadTester.Design
{
    public class Constant
    {
        public Constant() { }

        public Constant(string constantExpression, string value = null)
        {
            constantExpression = constantExpression.Split(' ')[0]; // skip constraints
            var parts = constantExpression.Split(':', StringSplitOptions.RemoveEmptyEntries);
            Overshadow = constantExpression.StartsWith(':');
            Name = parts[0];
            Value = value;
            if (parts.Length == 2)
            {
                var types = parts[1];
                var typeParts = types.Split("->");
                Type = typeParts[0];
                Conversions = typeParts.Skip(1).ToArray();
            }
        }

        public string Name { get; set; }
        public bool Overshadow { get; set; }
        public string Value { get; set; }
        public string Type { get; set; } = "string";
        public string[] Conversions { get; set; } = Array.Empty<string>();

        public static Constant[] Merge(IEnumerable<Constant> defaults, IEnumerable<Constant> overrides)
            => defaults.Concat(overrides).GroupBy(c => c.Name).Select(Merge).ToArray();

        private static Constant Merge(IEnumerable<Constant> constants)
        {
            var arr = constants.ToArray();
            var first = arr.First();
            var last = arr.Last();
            return first == last ? first : Merge(first, last);
        }

        private static Constant Merge(Constant first, Constant last)
            => new()
            {
                Conversions = first.Conversions,
                Name = first.Name,
                Type = first.Type,
                Value = last.Value
            };
    }
}