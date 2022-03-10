using Applique.LoadTester.Logic.Environment.Test.Bindings;
using Xunit;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Environment.Test.Bindings;

public class WhenGet : TestBindings<object>
{
    protected override void Act() => CollectResult(() => SUT.Get(SomeConstant));

    public class GivenValueNotExist : WhenGet
    {
        [Fact]
        public void ThenReturnNull()
        {
            ArrangeAndAct();
            Assert.Null(Result);
        }
    }

    public class GivenStringExist : WhenGet
    {
        [Fact]
        public void ThenReturnString()
        {
            Variables[SomeConstant] = SomeString;
            ArrangeAndAct();
            Assert.Equal(SomeString, Result);
        }
    }

    public class GivenIntExist : WhenGet
    {
        [Fact]
        public void ThenReturnInt()
        {
            Variables[SomeConstant] = SomeInt;
            ArrangeAndAct();
            Assert.Equal(SomeInt, Result);
        }
    }

    public class GivenDecimalExist : WhenGet
    {
        [Fact]
        public void ThenReturnDecimal()
        {
            Variables[SomeConstant] = SomeDecimal;
            ArrangeAndAct();
            Assert.Equal(SomeDecimal, Result);
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
            Assert.Equal(SomeInt, Result);
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
            Assert.Equal(SomeString, Result);
        }
    }
}