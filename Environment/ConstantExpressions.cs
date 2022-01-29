using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Service;
using Newtonsoft.Json.Linq;
using System;

namespace Applique.LoadTester.Environment
{
    public static class ConstantExpressions
    {
        public const string START_CONSTANT = "{{";
        public const string END_CONSTANT = "}}";

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

        private static Constraint ParseConstraint(string str)
            => Enum.TryParse<Constraint>(str, out var val) ? val : default;
    }
}