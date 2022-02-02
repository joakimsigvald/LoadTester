using Applique.LoadTester.Core.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using static Applique.LoadTester.Environment.ConstantExpressions;

namespace Applique.LoadTester.Environment
{
    public static class ConstantFactory
    {
        public static bool TryGetConstant(string val, out Constant constant)
            => TryExtractConstant(val, out constant) && IsConstant(val);

        public static bool TryExtractConstant(string val, out Constant constant)
        {
            constant = default;
            var startIndex = val?.IndexOf(START_CONSTANT) ?? -1;
            var endIndex = val?.IndexOf(END_CONSTANT) ?? -1;
            if (startIndex < 0 || startIndex >= endIndex)
                return false;
            var str = val[(startIndex + 2)..endIndex];
            constant = Create(str);
            return true;
        }

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
            var expr = ExtractConstraint(constantExpression, out var constraint);
            var overshadow = expr.StartsWith(':');
            expr = ExtractTypes(expr, out var type, out var conversions);
            var name = ExtractTolerance(expr, out var tolerance);
            return new Constant()
            {
                Overshadow = overshadow,
                Name = name,
                Tolerance = tolerance,
                Type = type,
                Conversions = conversions,
                Constraint = constraint,
                Value = value
            };
        }

        private static string ExtractConstraint(string expr, out Constraint constraint)
        {
            var parts = expr.Split(' ');
            constraint = ParseConstraint(parts[1..].FirstOrDefault());
            return parts[0];
        }

        private static string ExtractTypes(string expr, out ConstantType type, out ConstantType[] conversions)
        {
            var parts = expr.Split(':', StringSplitOptions.RemoveEmptyEntries);
            var types = GetTypes(parts[1..].FirstOrDefault());
            type = types[0];
            conversions = types[1..];
            return parts[0];
        }

        private static string ExtractTolerance(string expr, out decimal tolerance)
        {
            var parts = expr.Split("+-");
            tolerance = GetTolerance(parts[1..].FirstOrDefault());
            return parts[0];
        }

        private static decimal GetTolerance(string expr) => expr is null ? 0 : decimal.Parse(expr);

        private static ConstantType[] GetTypes(string expr)
            => expr is null 
            ? new[] { ConstantType.String} 
            : expr.Split("->").Select(Enum.Parse<ConstantType>).ToArray();

        public static Constant[] Merge(IEnumerable<Constant> defaults, IEnumerable<Constant> overrides)
            => defaults.Concat(overrides).GroupBy(c => c.Name).Select(Merge).ToArray();

        private static Constraint ParseConstraint(string str)
            => string.IsNullOrEmpty(str) ? default : Enum.Parse<Constraint>(str);

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