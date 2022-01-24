using System.Collections.Generic;
using System.Globalization;
using Applique.LoadTester.Domain.Environment;
using Newtonsoft.Json.Linq;
using Xunit;
using static Applique.LoadTester.Environment.ConstantExpressions;

namespace Applique.LoadTester.Environment.Test.Bindings
{
    public class WhenVerifyValue
    {
        protected const string _constantName = "SomeConstant";
        protected const string _propertyName = "SomeProperty";

        protected string Prefix = string.Empty;
        protected JProperty Expected;
        protected object ExpectedValue;
        protected string ActualValue = "SomeValue";
        protected IDictionary<string, object> Variables = new Dictionary<string, object>();

        protected Environment.Bindings Target { get; private set; }

        protected void Setup()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Expected = new JProperty(_propertyName, ExpectedValue);
            Target = new Environment.Bindings(new BindingVariables(Variables));
        }

        protected void Act() => Target.VerifyValue(Prefix, Expected, ActualValue);

        public class GivenValueExistAndEqual : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                ExpectedValue = Embrace(_constantName);
                Variables[_constantName] = ActualValue;
                Setup();
                Act();
            }
        }

        public class GivenValueExistAndNotEqual : WhenVerifyValue
        {
            [Fact]
            public void ThenThrowVerificationFailed()
            {
                ExpectedValue = Embrace(_constantName);
                Variables[_constantName] = $"Not {ActualValue}";
                Setup();
                Assert.Throws<VerificationFailed>(Act);
            }
        }

        public class GivenValueNotExistAndNotConstrained : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                ExpectedValue = Embrace(_constantName);
                Setup();
                Act();
            }
        }

        public class GivenValueExistButOvershadowedAndNotConstrained : WhenVerifyValue
        {
            [Fact]
            public void ThenPass()
            {
                ExpectedValue = Embrace($":{_constantName}");
                Variables[_constantName] = $"Not {ActualValue}";
                Setup();
                Act();
            }
        }

        public class GivenValueNotExistButConstraintViolated : WhenVerifyValue
        {
            [Fact]
            public void ThenThrowVerificationFailed()
            {
                ExpectedValue = Embrace($"{_constantName} {Constraint.Mandatory}");
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
                ExpectedValue = Embrace($"{_constantName} {Constraint.Mandatory}");
                ActualValue = "Not empty";
                Setup();
                Act();
            }
        }

        public class GivenDecimalExistButSlightlyDifferentAndNoTolerance : WhenVerifyValue
        {
            [Fact]
            public void ThenThrowVerificationFailed()
            {
                ExpectedValue = Embrace(_constantName);
                Variables[_constantName] = 10M;
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
                ExpectedValue = Embrace($"{_constantName}+-0.01");
                Variables[_constantName] = 10M;
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
                ExpectedValue = Embrace($"{_constantName}+-0.01");
                Variables[_constantName] = 10M;
                ActualValue = "10.02";
                Setup();
                Assert.Throws<VerificationFailed>(Act);
            }
        }
    }
}