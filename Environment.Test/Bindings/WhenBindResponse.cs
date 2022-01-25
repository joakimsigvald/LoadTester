using Newtonsoft.Json.Linq;
using Xunit;
using static Applique.LoadTester.Environment.ConstantExpressions;

namespace Applique.LoadTester.Environment.Test.Bindings
{
    public class WhenBindResponse : BindingsTestBase
    {
        protected readonly dynamic Pattern = new JObject();
        protected readonly dynamic ResponseToken = new JObject();

        protected void Act() => Target.BindResponse((JToken)Pattern, (JToken)ResponseToken);

        public class GivenStringNotExist : WhenBindResponse
        {
            [Fact]
            public void ThenSetString()
            {
                Pattern.SomeProperty = Embrace(ConstantName);
                ResponseToken.SomeProperty = SomeString;
                Setup();
                Act();
                var res = Target.Get(ConstantName);
                Assert.Same(SomeString, res);
            }
        }

        public class GivenIntNotExist : WhenBindResponse
        {
            [Fact]
            public void ThenSetInt()
            {
                Pattern.SomeProperty = Embrace($"{ConstantName}:int");
                ResponseToken.SomeProperty = SomeInt;
                Setup();
                Act();
                var res = Target.Get(ConstantName);
                Assert.Equal(SomeInt, (int)res);
            }
        }
    }
}