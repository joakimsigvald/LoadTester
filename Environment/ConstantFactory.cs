using Applique.LoadTester.Domain.Design;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Applique.LoadTester.Environment
{
    internal static class ConstantFactory
    {
        /// <summary>
        /// Constant -> (:)[Name](:[Type])(+-[Tolerance])( [Constraint])
        /// Name -> /String/
        /// Type -> int|decimal|string|dateTime
        /// Tolerance -> /Decimal/
        /// Constraint -> Mandatory
        /// </summary>
        public static Constant Create(string constantExpression, string value = null)
        {
            constantExpression = constantExpression.Split(' ')[0]; // skip constraints
            var parts = constantExpression.Split(':', StringSplitOptions.RemoveEmptyEntries);
            var subParts = parts[0].Split("+-");
            var res = new Constant()
            {
                Overshadow = constantExpression.StartsWith(':'),
                Name = subParts[0],
                Value = value
            };
            if (parts.Length == 2)
            {
                var types = parts[1];
                var typeParts = types.Split("->");
                res.Type = typeParts[0];
                res.Conversions = typeParts.Skip(1).ToArray();
            }
            if (subParts.Length == 2)
                res.Tolerance = decimal.Parse(subParts[1]);
            return res;
        }

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