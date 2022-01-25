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

        protected override void Arrange() => Expected = new JProperty(_propertyName, ExpectedValue);

        protected void Act() => Target.VerifyValue(Prefix, Expected, ActualValue);

        public class GivenValueExistAndEqual : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                ExpectedValue = Embrace(ConstantName);
                Variables[ConstantName] = ActualValue;
                Setup();
                Act();
            }
        }

        public class GivenValueExistAndNotEqual : WhenVerifyValue
        {
            [Fact]
            public void ThenThrowVerificationFailed()
            {
                ExpectedValue = Embrace(ConstantName);
                Variables[ConstantName] = $"Not {ActualValue}";
                Setup();
                Assert.Throws<VerificationFailed>(Act);
            }
        }

        public class GivenValueNotExistAndNotConstrained : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                ExpectedValue = Embrace(ConstantName);
                Setup();
                Act();
            }
        }

        public class GivenValueExistButOvershadowedAndNotConstrained : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                ExpectedValue = Embrace($":{ConstantName}");
                Variables[ConstantName] = $"Not {ActualValue}";
                Setup();
                Act();
            }
        }

        public class GivenValueNotExistButConstraintViolated : WhenVerifyValue
        {
            [Fact]
            public void ThenThrowVerificationFailed()
            {
                ExpectedValue = Embrace($"{ConstantName} {Constraint.Mandatory}");
                ActualValue = null;
                Setup();
                Assert.Throws<VerificationFailed>(Act);
            }
        }

        public class GivenValueNotExistAndConstraintFollowed : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                ExpectedValue = Embrace($"{ConstantName} {Constraint.Mandatory}");
                ActualValue = "Not empty";
                Setup();
                Act();
            }
        }

        public class GivenValueExistAndIsEmbeddedInString : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                var embeddedString = "embedded";
                Variables[ConstantName] = embeddedString;
                ActualValue = $"before {embeddedString} after";
                ExpectedValue = $"before {Embrace(ConstantName)} after";
                Setup();
                Act();
            }
        }

        public class GivenDecimalExistButSlightlyDifferentAndNoTolerance : WhenVerifyValue
        {
            [Fact]
            public void ThenThrowVerificationFailed()
            {
                ExpectedValue = Embrace(ConstantName);
                Variables[ConstantName] = 10M;
                ActualValue = "10.01";
                Setup();
                Assert.Throws<VerificationFailed>(Act);
            }
        }

        public class GivenDecimalExistAndWithinTolerance : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                ExpectedValue = Embrace($"{ConstantName}+-0.01");
                Variables[ConstantName] = 10M;
                ActualValue = "10.01";
                Setup();
                Act();
            }
        }

        public class GivenDecimalExistAndNotWithinTolerance : WhenVerifyValue
        {
            [Fact]
            public void ThenThrowVerificationFailed()
            {
                ExpectedValue = Embrace($"{ConstantName}+-0.01");
                Variables[ConstantName] = 10M;
                ActualValue = "10.02";
                Setup();
                Assert.Throws<VerificationFailed>(Act);
            }
        }
    }
}