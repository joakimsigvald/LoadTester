﻿using Xunit;
using static Applique.LoadTester.Environment.ConstantExpressions;
using static Applique.LoadTester.Environment.Test.TestData;

namespace Applique.LoadTester.Environment.Test.ValueVerifier
{
    public class WhenVerifyEquation : ValueVerifierTestBase
    {
        public class GivenTemplateIsSumOfDecimalsEqualToValue : WhenVerifyValue
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

        public class GivenTemplateIsSumOfIntsEqualToValue : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                TemplateValue = $"={Embrace(SomeConstant)}+{Embrace(AnotherConstant)}";
                Variables[SomeConstant] = SomeInt;
                Variables[AnotherConstant] = AnotherInt;
                Value = $"{SomeInt + AnotherInt}";
                ArrangeAndAct();
            }
        }

        public class GivenTemplateIsSumDifferentFromValue : WhenVerifyValue
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