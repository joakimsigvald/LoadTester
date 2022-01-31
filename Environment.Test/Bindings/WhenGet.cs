using Xunit;
using static Applique.LoadTester.Environment.Test.TestData;

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

        public class GivenStringOverloadedWithInt : WhenGet
        {
            [Fact]
            public void ThenReturnInt()
            {
                Variables[SomeConstant] = SomeString;
                OverloadVariables[SomeConstant] = SomeInt;
                ArrangeAndAct();
                Assert.Equal(SomeInt, ReturnValue);
            }
        }

        public class GivenStringWasOverloadedWithIntButOverloadRemoved : WhenGet
        {
            [Fact]
            public void ThenReturnString()
            {
                Variables[SomeConstant] = SomeString;
                OverloadVariables[SomeConstant] = SomeInt;
                Arrange();
                SUT.OverloadWith(null);
                Act();
                Assert.Equal(SomeString, ReturnValue);
            }
        }
    }
}