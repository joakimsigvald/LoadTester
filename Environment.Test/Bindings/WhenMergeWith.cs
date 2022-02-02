using Applique.LoadTester.Core.Service;
using Moq;
using System.Collections.Generic;
using Xunit;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Environment.Test.Bindings
{
    public class WhenMergeWithThenGet : BindingsTestBase<object>
    {
        protected IBindings OtherBindings;
        protected IDictionary<string, object> OtherVariables = new Dictionary<string, object>();

        protected override void Given() 
            => OtherBindings = Mock.Of<IBindings>(bindings => bindings.Variables == OtherVariables);

        protected override void Act()
        {
            SUT.MergeWith(OtherBindings);
            ReturnValue = SUT.Get(SomeConstant);
        }

        public class GivenHasValueAndOtherIsEmpty : WhenMergeWithThenGet
        {
            [Fact]
            public void ThenReturnOriginal()
            {
                Variables[SomeConstant] = SomeString;
                ArrangeAndAct();
                Assert.Equal(SomeString, ReturnValue);
            }
        }

        public class GivenHasValueAndOtherHasOtherConstant : WhenMergeWithThenGet
        {
            [Fact]
            public void ThenReturnOriginal()
            {
                Variables[SomeConstant] = SomeString;
                OtherVariables[AnotherConstant] = AnotherString;
                ArrangeAndAct();
                Assert.Equal(SomeString, ReturnValue);
            }
        }

        public class GivenHasValueAndOtherHasOtherValue : WhenMergeWithThenGet
        {
            [Fact]
            public void ThenReturnOtherValue()
            {
                Variables[SomeConstant] = SomeString;
                OtherVariables[SomeConstant] = AnotherString;
                ArrangeAndAct();
                Assert.Equal(AnotherString, ReturnValue);
            }
        }

        public class GivenOnlyOtherHasValue : WhenMergeWithThenGet
        {
            [Fact]
            public void ThenReturnOtherValue()
            {
                OtherVariables[SomeConstant] = AnotherString;
                ArrangeAndAct();
                Assert.Equal(AnotherString, ReturnValue);
            }
        }
    }
}