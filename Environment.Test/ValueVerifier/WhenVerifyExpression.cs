using Xunit;
using static Applique.LoadTester.Environment.ConstantExpressions;
using static Applique.LoadTester.Environment.Test.TestData;

namespace Applique.LoadTester.Environment.Test.ValueVerifier
{
    public class WhenVerifyExpression : ValueVerifierTestBase
    {
        public class GivenTemplateIsSumExpressionEqualToValue : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                TemplateValue = $"={Embrace(SomeConstant)}+{Embrace(AnotherConstant)}";
                Variables[SomeConstant] = SomeDecimal;
                Variables[AnotherConstant] = AnotherDecimal;
                Value = $"{SomeDecimal + AnotherDecimal}";
                ArrangeAndAct();
            }
        }

        public class GivenTemplateIsSumExpressionDifferentFromValue : WhenVerifyValue
        {
            [Fact]
            public void ThenThrowVerificationFailed()
            {
                TemplateValue = $"={Embrace(SomeConstant)}+{Embrace(AnotherConstant)}";
                Variables[SomeConstant] = SomeDecimal;
                Variables[AnotherConstant] = AnotherDecimal;
                Value = $"{SomeDecimal + AnotherDecimal + 1}";
                Arrange();
                Assert.Throws<VerificationFailed>(Act);
            }
        }
    }
}