using Applique.LoadTester.Logic.Environment;
using Applique.LoadTester.Logic.Environment.Test.Bindings;
using System;
using Xunit;
using static Applique.LoadTester.Domain.Design.ConstantExpressions;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Environment.Test.Bindings;

public class WhenSubstituteVariables : TestBindings<string>
{
    protected string Target;

    protected override void Act() => CollectResult(() => SUT.SubstituteVariables(Target));

    public class GivenNoConstantExpressionsInTarget : WhenSubstituteVariables
    {
        [Fact]
        public void ThenReturnTarget()
        {
            Target = SomeString;
            ArrangeAndAct();
            Assert.Equal(Target, Result);
        }
    }

    public class GivenConstantInTargetNotExist : WhenSubstituteVariables
    {
        [Fact]
        public void ThenReturnTarget()
        {
            Target = $"A{Embrace(SomeConstant)}B";
            ArrangeAndAct();
            Assert.Equal(Target, Result);
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
            Assert.Equal($"A{SomeString}B", Result);
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
            Assert.Equal($"A{AnotherString}B", Result);
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
            Assert.Equal($"{SomeInt}", Result);
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
            Assert.Equal($"{SomeInt}", Result);
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
            Assert.Equal($"{SomeInt}", Result);
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
            Assert.Equal($"{SomeDecimal}", Result);
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
            Assert.Equal($"{SomeDecimal}", Result);
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
            Assert.Equal("true", Result);
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
            var currentTime = DateTime.Parse(Result);
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
            Assert.Equal(Target, Result);
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
            Assert.Equal(Target, Result);
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
            Assert.Equal(Target, Result);
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
            Assert.Equal(Target, Result);
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
            Assert.Equal($"A{SomeString}B{SomeInt}C", Result);
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
            Assert.Equal($"A{SomeString}B{SomeString}C", Result);
        }
    }
}