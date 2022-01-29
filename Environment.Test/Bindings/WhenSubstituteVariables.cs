using Xunit;
using static Applique.LoadTester.Environment.ConstantExpressions;

namespace Applique.LoadTester.Environment.Test.Bindings
{
    public class WhenSubstituteVariables : BindingsTestBase<object>
    {
        protected string Target;

        protected override void Act() => ReturnValue = SUT.SubstituteVariables(Target);

        public class GivenNoConstantExpressionsInTarget : WhenSubstituteVariables
        {
            [Fact]
            public void ThenReturnTarget()
            {
                Target = SomeString;
                ArrangeAndAct();
                Assert.Equal(Target, ReturnValue);
            }
        }

        public class GivenConstantInTargetNotExist : WhenSubstituteVariables
        {
            [Fact]
            public void ThenReturnTarget()
            {
                Target = $"{SomeInt}{START_CONSTANT}{SomeConstant}{END_CONSTANT}{AnotherInt}";
                ArrangeAndAct();
                Assert.Equal(Target, ReturnValue);
            }
        }

        public class GivenConstantInTargetExist : WhenSubstituteVariables
        {
            [Fact]
            public void ThenSubstituteConstantWithValue()
            {
                Variables[SomeConstant] = SomeString;
                Target = $"{SomeInt}{START_CONSTANT}{SomeConstant}{END_CONSTANT}{AnotherInt}";
                ArrangeAndAct();
                Assert.Equal($"{SomeInt}{SomeString}{AnotherInt}", ReturnValue);
            }
        }

        public class GivenConstantInTargetIsOverloaded : WhenSubstituteVariables
        {
            [Fact]
            public void ThenSubstituteConstantWithOverloadedValue()
            {
                Variables[SomeConstant] = SomeString;
                OverloadVariables[SomeConstant] = AnotherString;
                Target = $"{SomeInt}{START_CONSTANT}{SomeConstant}{END_CONSTANT}{AnotherInt}";
                Arrange();
                SUT.OverloadWith(CreateBindings(OverloadVariables));
                Act();
                Assert.Equal($"{SomeInt}{AnotherString}{AnotherInt}", ReturnValue);
            }
        }
    }
}