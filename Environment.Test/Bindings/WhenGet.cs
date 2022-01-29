using Xunit;

namespace Applique.LoadTester.Environment.Test.Bindings
{

    public class WhenGet : BindingsTestBase<object>
    {
        protected override void Act() => ReturnValue = SUT.Get(SomeConstant);

        public class GivenValueNotExist : WhenGet
        {
            [Fact]
            public void ThenReturnNull()
            {
                ArrangeAndAct();
                Assert.Null(ReturnValue);
            }
        }

        public class GivenStringExist : WhenGet
        {
            [Fact]
            public void ThenReturnString()
            {
                Variables[SomeConstant] = SomeString;
                ArrangeAndAct();
                Assert.Equal(SomeString, ReturnValue);
            }
        }

        public class GivenIntExist : WhenGet
        {
            [Fact]
            public void ThenReturnInt()
            {
                Variables[SomeConstant] = SomeInt;
                ArrangeAndAct();
                Assert.Equal(SomeInt, ReturnValue);
            }
        }

        public class GivenDecimalExist : WhenGet
        {
            [Fact]
            public void ThenReturnDecimal()
            {
                Variables[SomeConstant] = SomeDecimal;
                ArrangeAndAct();
                Assert.Equal(SomeDecimal, ReturnValue);
            }
        }
    }
}