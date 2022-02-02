using Applique.LoadTester.Core.Design;
using Xunit;
using static Applique.LoadTester.Environment.ConstantExpressions;
using static Applique.LoadTester.Environment.Test.TestData;

namespace Applique.LoadTester.Environment.Test.ValueVerifier
{
    public class WhenVerifyValue : ValueVerifierTestBase
    {
        public class GivenConstantExistAndEqualToValue : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                TemplateValue = Embrace(SomeConstant);
                Variables[SomeConstant] = Value;
                ArrangeAndAct();
            }
        }

        public class GivenConstantExistAndNotEqualtoValue : WhenVerifyValue
        {
            [Fact]
            public void ThenThrowVerificationFailed()
            {
                TemplateValue = Embrace(SomeConstant);
                Variables[SomeConstant] = $"Not {Value}";
                Arrange();
                Assert.Throws<VerificationFailed>(Act);
            }
        }

        public class GivenConstantNotExistAndNotConstrained : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                TemplateValue = Embrace(SomeConstant);
                ArrangeAndAct();
            }
        }

        public class GivenConstantExistButOvershadowedAndNotConstrained : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                TemplateValue = Embrace($":{SomeConstant}");
                Variables[SomeConstant] = $"Not {Value}";
                ArrangeAndAct();
            }
        }

        public class GivenConstantNotExistButConstraintViolated : WhenVerifyValue
        {
            [Fact]
            public void ThenThrowVerificationFailed()
            {
                TemplateValue = Embrace($"{SomeConstant} {Constraint.Mandatory}");
                Value = null;
                Arrange();
                Assert.Throws<VerificationFailed>(Act);
            }
        }

        public class GivenConstantNotExistAndConstraintFollowed : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                TemplateValue = Embrace($"{SomeConstant} {Constraint.Mandatory}");
                Value = "Not empty";
                ArrangeAndAct();
            }
        }

        public class GivenConstantExistAndIsEmbeddedInString : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                var embeddedString = "embedded";
                Variables[SomeConstant] = embeddedString;
                TemplateValue = $"before {Embrace(SomeConstant)} after";
                Value = $"before {embeddedString} after";
                ArrangeAndAct();
            }
        }

        public class GivenDecimalExistButSlightlyDifferentAndNoTolerance : WhenVerifyValue
        {
            [Fact]
            public void ThenThrowVerificationFailed()
            {
                TemplateValue = Embrace(SomeConstant);
                Variables[SomeConstant] = 10M;
                Value = "10.01";
                Arrange();
                Assert.Throws<VerificationFailed>(Act);
            }
        }

        public class GivenDecimalExistAndWithinTolerance : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                TemplateValue = Embrace($"{SomeConstant}+-0.01");
                Variables[SomeConstant] = 10M;
                Value = "10.01";
                ArrangeAndAct();
            }
        }

        public class GivenDecimalExistAndNotWithinTolerance : WhenVerifyValue
        {
            [Fact]
            public void ThenThrowVerificationFailed()
            {
                TemplateValue = Embrace($"{SomeConstant}+-0.01");
                Variables[SomeConstant] = 10M;
                Value = "10.02";
                Arrange();
                Assert.Throws<VerificationFailed>(Act);
            }
        }
    }
}