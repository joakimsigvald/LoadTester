﻿using Newtonsoft.Json.Linq;
using Xunit;
using static Applique.LoadTester.Environment.ConstantExpressions;

namespace Applique.LoadTester.Environment.Test.Bindings
{
    public class WhenBindResponse : BindingsTestBase
    {
        protected readonly dynamic Pattern = new JObject();
        protected readonly dynamic ResponseToken = new JObject();

        protected override void Act() => SUT.BindResponse((JToken)Pattern, (JToken)ResponseToken);

        public class GivenStringNotExist : WhenBindResponse
        {
            [Fact]
            public void ThenSetString()
            {
                Pattern.SomeProperty = Embrace(SomeConstant);
                ResponseToken.SomeProperty = SomeString;
                ArrangeAndAct();
                var res = SUT.Get(SomeConstant);
                Assert.Same(SomeString, res);
            }
        }

        public class GivenIntExistAndUntypedPattern : WhenBindResponse
        {
            [Fact]
            public void ThenReplaceValueButKeepType()
            {
                Variables[SomeConstant] = SomeInt;
                Pattern.SomeProperty = Embrace(SomeConstant);
                ResponseToken.SomeProperty = AnotherInt;
                ArrangeAndAct();
                var res = SUT.Get(SomeConstant);
                Assert.Equal(AnotherInt, res);
            }
        }

        public class GivenStringExistButOvershadowed : WhenBindResponse
        {
            [Fact]
            public void ThenReplaceValueAndType()
            {
                Variables[SomeConstant] = SomeInt;
                Pattern.SomeProperty = Embrace($":{SomeConstant}");
                ResponseToken.SomeProperty = AnotherInt;
                ArrangeAndAct();
                var res = SUT.Get(SomeConstant);
                Assert.Equal($"{AnotherInt}", res);
            }
        }

        public class GivenIntNotExist : WhenBindResponse
        {
            [Fact]
            public void ThenSetInt()
            {
                Pattern.SomeProperty = Embrace($"{SomeConstant}:int");
                ResponseToken.SomeProperty = SomeInt;
                ArrangeAndAct();
                var res = SUT.Get(SomeConstant);
                Assert.Equal(SomeInt, (int)res);
            }
        }

        public class GivenDecimalNotExist : WhenBindResponse
        {
            [Fact]
            public void ThenSetDecimal()
            {
                Pattern.SomeProperty = Embrace($"{SomeConstant}:decimal");
                ResponseToken.SomeProperty = SomeDecimal;
                ArrangeAndAct();
                var res = SUT.Get(SomeConstant);
                Assert.Equal(SomeDecimal, (decimal)res);
            }
        }
    }
}