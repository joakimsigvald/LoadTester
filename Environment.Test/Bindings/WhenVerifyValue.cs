using Applique.LoadTester.Domain.Environment;
using Newtonsoft.Json.Linq;
using Xunit;
using static Applique.LoadTester.Environment.ConstantExpressions;

namespace Applique.LoadTester.Environment.Test.Bindings
{
    public class WhenVerifyValue : BindingsTestBase
    {
        protected const string _propertyName = "SomeProperty";

        protected string Prefix = string.Empty;
        protected JProperty Expected;
        protected object ExpectedValue;
        protected string ActualValue = SomeString;

        protected override void Given() => Expected = new JProperty(_propertyName, ExpectedValue);
        protected override void Act() => SUT.VerifyValue(Prefix, Expected, ActualValue);

        public class GivenValueExistAndEqual : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                ExpectedValue = Embrace(SomeConstant);
                Variables[SomeConstant] = ActualValue;
                ArrangeAndAct();
            }
        }

        public class GivenValueExistAndNotEqual : WhenVerifyValue
        {
            [Fact]
            public void ThenThrowVerificationFailed()
            {
                ExpectedValue = Embrace(SomeConstant);
                Variables[SomeConstant] = $"Not {ActualValue}";
                Arrange();
                Assert.Throws<VerificationFailed>(Act);
            }
        }

        public class GivenValueNotExistAndNotConstrained : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                ExpectedValue = Embrace(SomeConstant);
                ArrangeAndAct();
            }
        }

        public class GivenValueExistButOvershadowedAndNotConstrained : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                ExpectedValue = Embrace($":{SomeConstant}");
                Variables[SomeConstant] = $"Not {ActualValue}";
                ArrangeAndAct();
            }
        }

        public class GivenValueNotExistButConstraintViolated : WhenVerifyValue
        {
            [Fact]
            public void ThenThrowVerificationFailed()
            {
                ExpectedValue = Embrace($"{SomeConstant} {Constraint.Mandatory}");
                ActualValue = null;
                Arrange();
                Assert.Throws<VerificationFailed>(Act);
            }
        }

        public class GivenValueNotExistAndConstraintFollowed : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                ExpectedValue = Embrace($"{SomeConstant} {Constraint.Mandatory}");
                ActualValue = "Not empty";
                ArrangeAndAct();
            }
        }

        public class GivenValueExistAndIsEmbeddedInString : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                var embeddedString = "embedded";
                Variables[SomeConstant] = embeddedString;
                ActualValue = $"before {embeddedString} after";
                ExpectedValue = $"before {Embrace(SomeConstant)} after";
                ArrangeAndAct();
            }
        }

        public class GivenDecimalExistButSlightlyDifferentAndNoTolerance : WhenVerifyValue
        {
            [Fact]
            public void ThenThrowVerificationFailed()
            {
                ExpectedValue = Embrace(SomeConstant);
                Variables[SomeConstant] = 10M;
                ActualValue = "10.01";
                Arrange();
                Assert.Throws<VerificationFailed>(Act);
            }
        }

        public class GivenDecimalExistAndWithinTolerance : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                ExpectedValue = Embrace($"{SomeConstant}+-0.01");
                Variables[SomeConstant] = 10M;
                ActualValue = "10.01";
                ArrangeAndAct();
            }
        }

        public class GivenDecimalExistAndNotWithinTolerance : WhenVerifyValue
        {
            [Fact]
            public void ThenThrowVerificationFailed()
            {
                ExpectedValue = Embrace($"{SomeConstant}+-0.01");
                Variables[SomeConstant] = 10M;
                ActualValue = "10.02";
                Arrange();
                Assert.Throws<VerificationFailed>(Act);
            }
        }
    }
}