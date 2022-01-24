using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Environment;
using Newtonsoft.Json.Linq;
using System;

namespace Applique.LoadTester.Environment
{
    public static class ConstantExpressions
    {
        public static bool TryGetVariableName(JProperty p, out string varName)
        {
            varName = null;
            if (!IsString(p))
                return false;
            var val = p.Value.Value<string>();
            if (IsVariable(val))
                varName = ConstantFactory.Create(Unembrace(val)).Name;
            return true;
        }

        public static bool TryExtractConstant(string val, out Constant constant)
        {
            constant = default;
            var startIndex = val?.IndexOf("{{") ?? -1;
            var endIndex = val?.IndexOf("}}") ?? -1;
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
            var startIndex = target.IndexOf("{{");
            var endIndex = target.IndexOf("}}");
            return target[..startIndex] + value + target[(endIndex + 2)..];
        }

        public static string Embrace(string value) => "{{" + value + "}}";

        public static string Unembrace(string variable) => variable[2..^2];

        public static bool IsVariable(string val)
            => val?.StartsWith("{{") == true && val?.EndsWith("}}") == true;

        public static bool IsString(JProperty p)
            => p.Value.Type == JTokenType.String;

        private static Constraint ParseConstraint(string str)
            => Enum.TryParse<Constraint>(str, out var val) ? val : default;
    }
}