using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Service;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace Applique.LoadTester.Environment
{
    public static class ConstantExpressions
    {
        public const string START_CONSTANT = "{{";
        public const string END_CONSTANT = "}}";

        public static object ValueOf(Constant constant)
        {
            var value = ParseValue(constant);
            var from = constant.Type;
            foreach (var conversion in constant.Conversions)
            {
                value = Cast(value, from, conversion);
                from = conversion;
            }
            return value;
        }

        public static ConstantType TypeOf(object obj)
            => obj is int? ? ConstantType.Int
            : obj is bool? ? ConstantType.Bool
            : obj is decimal? ? ConstantType.Decimal
            : obj is DateTime? ? ConstantType.DateTime
            : ConstantType.String;

        public static bool TryGetConstant(string val, out Constant constant) 
            => TryExtractConstant(val, out constant) && IsVariable(val);

        public static bool TryExtractConstant(string val, out Constant constant)
        {
            constant = default;
            var startIndex = val?.IndexOf(START_CONSTANT) ?? -1;
            var endIndex = val?.IndexOf(END_CONSTANT) ?? -1;
            if (startIndex < 0 || startIndex >= endIndex)
                return false;
            var str = val[(startIndex + 2)..endIndex];
            constant = ConstantFactory.Create(str);
            return true;
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

        public static string ReplaceConstantExpressionWithValue(string target, string value)
        {
            var startIndex = target.IndexOf(START_CONSTANT);
            var endIndex = target.IndexOf(END_CONSTANT);
            return target[..startIndex] + value + target[(endIndex + 2)..];
        }

        public static string Embrace(string value) => START_CONSTANT + value + END_CONSTANT;

        public static string Unembrace(string variable) => variable[2..^2];

        public static bool IsVariable(string val)
            => val?.StartsWith(START_CONSTANT) == true && val?.EndsWith(END_CONSTANT) == true;

        public static bool IsString(JProperty p)
            => p.Value.Type == JTokenType.String;

        public static bool IsBool(object obj)
            => obj is bool?;

        public static bool IsScalar(object obj)
            => obj is int? || obj is decimal? || obj is bool?;

        private static object Cast(object value, ConstantType from, ConstantType to)
            => to switch
            {
                ConstantType.Int when from == ConstantType.Decimal => decimal.ToInt32((decimal)value),
                _ => throw new InvalidOperationException($"Cannot cast {value} from {from} to {to}")
            };

        private static object ParseValue(Constant constant)
            => constant.Value is null
            ? null
            : constant.Type switch
            {
                ConstantType.DateTime => DateTime.Parse(constant.Value),
                ConstantType.Int => int.Parse(constant.Value),
                ConstantType.Bool => bool.Parse(constant.Value),
                ConstantType.Decimal => decimal.Parse(constant.Value, CultureInfo.InvariantCulture),
                _ => constant.Value,
            };

        private static Constraint ParseConstraint(string str)
            => Enum.TryParse<Constraint>(str, out var val) ? val : default;
    }
}