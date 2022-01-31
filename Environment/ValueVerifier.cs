using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Service;
using Newtonsoft.Json.Linq;
using System.Linq;
using static Applique.LoadTester.Environment.ConstantExpressions;

namespace Applique.LoadTester.Environment
{
    public class ValueVerifier : IValueVerifier
    {
        private readonly IBindings _bindings;

        public ValueVerifier(IBindings bindings) => _bindings = bindings;

        public void VerifyValue(string prefix, JProperty expected, string actualValue)
        {
            if (!TrySubstituteVariable(expected.Value?.ToString(), out var expectedValue))
                CheckConstraints(prefix, GetConstraint(expected), actualValue);
            else if (!IsMatch(expectedValue, actualValue))
                throw new VerificationFailed(prefix, $"Unexpected response: {actualValue}, expected {expectedValue}");
        }

        private static bool IsMatch(object expectedValue, string actualValue)
            => expectedValue is DecimalWithTolerance decObj ? decObj.IsMatch(
                    ValueRetriever.ValueOf(new Constant { Value = actualValue, Type = "decimal" }) as decimal?)
            : $"{expectedValue}" == actualValue?.ToString();

        private static void CheckConstraints(string property, Constraint constraint, string actualValue)
        {
            switch (constraint)
            {
                case Constraint.Mandatory:
                    if (string.IsNullOrEmpty(actualValue))
                        throw new VerificationFailed(property, $"Constrain violated: {constraint}, value: {actualValue}");
                    break;
                default: break;
            }
        }

        private bool TrySubstituteVariable(string target, out object value)
        {
            value = target;
            if (TryExtractFormula(target, out var expr))
            {
                value = expr.Cast<decimal>().Aggregate(0M, (a, b) => a + b);
                return true;
            }
            if (!TryExtractConstant(target, out var constant))
                return true; // we successfully substituted all 0 variables in the expression (no constraints to check)
            if (constant.Overshadow)
                return false; // we didn't substitute because any stored value is disregarded and will be replaced (check constraints instead)
            if (!_bindings.TryGet(constant.Name, out var val))
                return false; // we didn't substitute because no value is stored yet (check constraints instead)
            value = !IsVariable(target)
                ? ReplaceConstantExpressionWithValue(target, $"{val}")
                : constant.Tolerance != 0
                ? new DecimalWithTolerance
                {
                    Value = val as decimal?,
                    Tolerance = constant.Tolerance
                }
                : val;
            return true; // we substitute existing variable value and will not store new value, so no need to check constraints
        }

        private bool TryExtractFormula(string target, out object[] terms)
        {
            terms = null;
            if (target?.StartsWith('=') != true)
                return false;
            terms = target[1..].Split('+').Select(Unembrace).Select(_bindings.Get).ToArray();
            return terms.Length > 1 && terms.All(t => t is decimal);
        }
    }
}