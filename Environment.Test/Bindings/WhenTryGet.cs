using Applique.LoadTester.Logic.Environment.Test.Bindings;
using Xunit;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Environment.Test.Bindings
{
    public class WhenTryGet : BindingsTestBase<bool?>
    {
        protected object Result;

        protected override void Act() => ReturnValue = SUT.TryGet(SomeConstant, out Result);

        public class GivenValueNotExist : WhenTryGet
        {
            [Fact]
            public void ThenReturnFalse()
            {
                ArrangeAndAct();
                Assert.False(ReturnValue);
            }
        }

        public class GivenStringExist : WhenTryGet
        {
            [Fact]
            public void ThenReturnTrueAndString()
            {
                Variables[SomeConstant] = SomeString;
                ArrangeAndAct();
                Assert.True(ReturnValue);
                Assert.Equal(SomeString, Result);
            }
        }

        public class GivenIntExist : WhenTryGet
        {
            [Fact]
            public void ThenReturnTrueAndInt()
            {
                Variables[SomeConstant] = SomeInt;
                Arrange();
                Act();
                Assert.True(ReturnValue);
                Assert.Equal(SomeInt, Result);
            }
        }

        public class GivenDecimalExist : WhenTryGet
        {
            [Fact]
            public void ThenReturnTrueAndDecimal()
            {
                Variables[SomeConstant] = SomeDecimal;
                Arrange();
                Act();
                Assert.True(ReturnValue);
                Assert.Equal(SomeDecimal, Result);
            }
        }
    }
}