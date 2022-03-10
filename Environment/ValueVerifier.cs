using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Service;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using static Applique.LoadTester.Domain.Design.ConstantExpressions;
using static Applique.LoadTester.Domain.Design.ConstantFactory;

namespace Applique.LoadTester.Logic.Environment;

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

    private static Constraint GetConstraint(JProperty pp)
        => IsString(pp) ? GetConstraint(pp.Value.Value<string>()) : default;

    private static Constraint GetConstraint(string expr)
        => IsConstant(expr) ? Create(Unembrace(expr)).Constraint : default;

    private static bool IsMatch(object expectedValue, string actualValue)
        => expectedValue is DecimalWithTolerance decObj
        ? decObj.IsMatch(ValueOf(
            new Constant { Value = actualValue, Type = ConstantType.Decimal }) as decimal?)
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
        if (TryExtractEquation(target, out var equation))
        {
            value = ComputeEquation(equation);
            return true;
        }
        if (!TryExtractConstant(target, out var constant))
            return true; // we successfully substituted all 0 variables in the expression (no constraints to check)
        if (constant.Overshadow)
            return false; // we didn't substitute because any stored value is disregarded and will be replaced (check constraints instead)
        if (!_bindings.TryGet(constant.Name, out var val))
            return false; // we didn't substitute because no value is stored yet (check constraints instead)
        value = !IsConstant(target)
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

    private bool TryExtractEquation(string target, out Equation equation)
    {
        equation = null;
        if (target?.StartsWith('=') != true)
            return false;
        var terms = target[1..].Split('+').Select(Unembrace).Select(_bindings.Get).ToArray();
        equation = new Equation { Type = TypeOf(terms[0]), Terms = terms };
        return true;
    }

    private static object ComputeEquation(Equation equation)
        => equation.Type switch
        {
            ConstantType.Int => equation.Terms.Cast<int>().Aggregate(0M, (a, b) => a + b),
            ConstantType.Decimal => equation.Terms.Cast<decimal>().Aggregate(0M, (a, b) => a + b),
            _ => throw new NotImplementedException($"Equation of type: {equation.Type}")
        };
}