using Applique.LoadTester.Logic.Environment.Test.Bindings;
using System;
using Xunit;
using static Applique.LoadTester.Domain.Service.ConstantExpressions;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Environment.Test.Bindings
{
    public class WhenSubstituteVariables : BindingsTestBase<string>
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
                Target = $"A{Embrace(SomeConstant)}B";
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
                Target = $"A{Embrace(SomeConstant)}B";
                ArrangeAndAct();
                Assert.Equal($"A{SomeString}B", ReturnValue);
            }
        }

        public class GivenConstantInTargetIsOverloaded : WhenSubstituteVariables
        {
            [Fact]
            public void ThenSubstituteConstantWithOverloadedValue()
            {
                Variables[SomeConstant] = SomeString;
                OverloadVariables[SomeConstant] = AnotherString;
                Target = $"A{Embrace(SomeConstant)}B";
                ArrangeAndAct();
                Assert.Equal($"A{AnotherString}B", ReturnValue);
            }
        }

        public class GivenTargetIsConstantBoundToInt : WhenSubstituteVariables
        {
            [Fact]
            public void ThenSubstituteConstantWithInt()
            {
                Variables[SomeConstant] = SomeInt;
                Target = Embrace(SomeConstant);
                ArrangeAndAct();
                Assert.Equal($"{SomeInt}", ReturnValue);
            }
        }

        public class GivenTargetIsQuotedConstantBoundToInt : WhenSubstituteVariables
        {
            [Fact]
            public void ThenSubstituteQuotedConstantWithInt()
            {
                Variables[SomeConstant] = SomeInt;
                Target = $"'{Embrace(SomeConstant)}'";
                ArrangeAndAct();
                Assert.Equal($"{SomeInt}", ReturnValue);
            }
        }

        public class GivenTargetIsDoubleQuotedConstantBoundToInt : WhenSubstituteVariables
        {
            [Fact]
            public void ThenSubstituteDoubleQuotedConstantWithInt()
            {
                Variables[SomeConstant] = SomeInt;
                Target = $"\"{Embrace(SomeConstant)}\"";
                ArrangeAndAct();
                Assert.Equal($"{SomeInt}", ReturnValue);
            }
        }

        public class GivenTargetIsQuotedConstantBoundToDecimal : WhenSubstituteVariables
        {
            [Fact]
            public void ThenSubstituteQuotedConstantWithDecimal()
            {
                Variables[SomeConstant] = SomeDecimal;
                Target = $"'{Embrace(SomeConstant)}'";
                ArrangeAndAct();
                Assert.Equal($"{SomeDecimal}", ReturnValue);
            }
        }

        public class GivenTargetIsConstantBoundToDecimal : WhenSubstituteVariables
        {
            [Fact]
            public void ThenSubstituteConstantWithDecimal()
            {
                Variables[SomeConstant] = SomeDecimal;
                Target = Embrace(SomeConstant);
                ArrangeAndAct();
                Assert.Equal($"{SomeDecimal}", ReturnValue);
            }
        }

        public class GivenTargetIsConstantBoundToBool : WhenSubstituteVariables
        {
            [Fact]
            public void ThenSubstituteConstantWithBool()
            {
                Variables[SomeConstant] = true;
                Target = Embrace(SomeConstant);
                ArrangeAndAct();
                Assert.Equal("true", ReturnValue);
            }
        }

        public class GivenTargetIsConstantBoundCurrentTime : WhenSubstituteVariables
        {
            [Fact]
            public void ThenSubstituteConstantWithCurrentTime()
            {
                var before = DateTime.Now.AddSeconds(-1);
                Variables[SomeConstant] = SpecialVariables.CurrentTime;
                Target = Embrace(SomeConstant);
                ArrangeAndAct();
                var currentTime = DateTime.Parse(ReturnValue);
                var after = DateTime.Now;
                Assert.True(currentTime >= before, $"Expected {currentTime.Ticks} >= {before.Ticks}");
                Assert.True(currentTime <= DateTime.Now, $"Expected {currentTime.Ticks} <= {after.Ticks}");
            }
        }

        public class GivenTargetIsOvershadowExistingConstant : WhenSubstituteVariables
        {
            [Fact]
            public void ThenDoNotSubstituteValue()
            {
                Variables[SomeConstant] = SomeString;
                Target = Embrace($":{SomeConstant}");
                ArrangeAndAct();
                Assert.Equal(Target, ReturnValue);
            }
        }

        public class GivenTargetIsTypedExistingConstant : WhenSubstituteVariables
        {
            [Fact]
            public void ThenDoNotSubstituteValue()
            {
                Variables[SomeConstant] = SomeString;
                Target = Embrace($"{SomeConstant}:int");
                ArrangeAndAct();
                Assert.Equal(Target, ReturnValue);
            }
        }

        public class GivenTargetIsExistingConstantWithTolerance : WhenSubstituteVariables
        {
            [Fact]
            public void ThenDoNotSubstituteValue()
            {
                Variables[SomeConstant] = SomeString;
                Target = Embrace($"{SomeConstant}+-0.1");
                ArrangeAndAct();
                Assert.Equal(Target, ReturnValue);
            }
        }

        public class GivenTargetIsExistingConstantWithConstraint : WhenSubstituteVariables
        {
            [Fact]
            public void ThenDoNotSubstituteValue()
            {
                Variables[SomeConstant] = SomeString;
                Target = Embrace($"{SomeConstant} Mandatory");
                ArrangeAndAct();
                Assert.Equal(Target, ReturnValue);
            }
        }

        public class GivenTwoConstantsInTargetExist : WhenSubstituteVariables
        {
            [Fact]
            public void ThenSubstituteBoth()
            {
                Variables[SomeConstant] = SomeString;
                Variables[AnotherConstant] = SomeInt;
                Target = $"A{Embrace(SomeConstant)}B{Embrace(AnotherConstant)}C";
                ArrangeAndAct();
                Assert.Equal($"A{SomeString}B{SomeInt}C", ReturnValue);
            }
        }

        public class GivenTargetHasExistingConstantTwice : WhenSubstituteVariables
        {
            [Fact]
            public void ThenSubstituteBothPlaces()
            {
                Variables[SomeConstant] = SomeString;
                Target = $"A{Embrace(SomeConstant)}B{Embrace(SomeConstant)}C";
                ArrangeAndAct();
                Assert.Equal($"A{SomeString}B{SomeString}C", ReturnValue);
            }
        }
    }
}