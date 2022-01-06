using Newtonsoft.Json.Linq;

namespace Applique.LoadTester.Business.Runtime
{
    public static class JPropertyExtensions
    {
        public static bool TryGetVariableName(this JProperty p, out string varName)
        {
            varName = null;
            if (!IsString(p))
                return false;
            var val = p.Value.Value<string>();
            if (IsVariable(val))
                varName = val[2..^2];
            return true;
        }

        public static bool TryGetValue(this JProperty p, out string value)
        {
            value = null;
            if (!IsString(p))
                return false;
            var val = p.Value.Value<string>();
            if (!IsVariable(val))
                value = val;
            return true;
        }

        private static bool IsVariable(string val)
            => val.StartsWith("{{") && val.EndsWith("}}");

        private static bool IsString(JProperty p)
            => p.Value.Type == JTokenType.String;
    }
}