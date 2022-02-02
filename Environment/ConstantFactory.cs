using Applique.LoadTester.Core.Design;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Applique.LoadTester.Environment
{
    public static class ConstantFactory
    {
        /// <summary>
        /// Constant -> (:)[Name](+-[Tolerance])(:[Type](->[Type]))( [Constraint])
        /// Name -> /String/
        /// Type -> int|decimal|string|dateTime
        /// Tolerance -> /Decimal/
        /// Constraint -> Mandatory
        /// </summary>
        public static Constant Create(string constantExpression, string value = null)
        {
            if (string.IsNullOrEmpty(constantExpression))
                return null;
            var superParts = constantExpression.Split(' ');
            var constraint = ParseConstraint(superParts.Skip(1).FirstOrDefault());
            var constantDefinition = superParts[0];
            var parts = constantDefinition.Split(':', StringSplitOptions.RemoveEmptyEntries);
            var subParts = parts[0].Split("+-");
            var res = new Constant()
            {
                Overshadow = constantDefinition.StartsWith(':'),
                Name = subParts[0],
                Value = value,
                Constraint = constraint
            };
            if (parts.Length == 2)
            {
                var types = parts[1];
                var typeParts = types.Split("->");
                res.Type = Enum.Parse<ConstantType>(typeParts[0]);
                res.Conversions = typeParts.Skip(1).Select(Enum.Parse<ConstantType>).ToArray();
            }
            if (subParts.Length == 2)
                res.Tolerance = decimal.Parse(subParts[1]);
            return res;
        }

        private static Constraint ParseConstraint(string str) 
            => string.IsNullOrEmpty(str) ? default : Enum.Parse<Constraint>(str);

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