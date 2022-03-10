using Applique.LoadTester.Logic.Environment.Test.Bindings;
using Newtonsoft.Json.Linq;
using Xunit;
using static Applique.LoadTester.Domain.Design.ConstantExpressions;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Environment.Test.Bindings
{
    public abstract class WhenBindResponse : TestBindings<object>
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
                Pattern.SomeProperty = Embrace($"{SomeConstant}:Int");
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
                Pattern.SomeProperty = Embrace($"{SomeConstant}:Decimal");
                ResponseToken.SomeProperty = SomeDecimal;
                ArrangeAndAct();
                var res = SUT.Get(SomeConstant);
                Assert.Equal(SomeDecimal, (decimal)res);
            }
        }
    }
}