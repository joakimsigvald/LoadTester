using Applique.LoadTester.Core.Design;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace Applique.LoadTester.Domain.Design
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

        public static string ReplaceConstantExpressionWithValue(string target, string value)
        {
            var startIndex = target.IndexOf(START_CONSTANT);
            var endIndex = target.IndexOf(END_CONSTANT);
            return target[..startIndex] + value + target[(endIndex + 2)..];
        }

        public static string Embrace(string value) => START_CONSTANT + value + END_CONSTANT;

        public static string Unembrace(string variable) => variable[2..^2];

        public static bool IsConstant(string val)
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
    }
}