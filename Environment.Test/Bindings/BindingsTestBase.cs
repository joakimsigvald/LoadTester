using Applique.LoadTester.Domain.Environment;
using System.Collections.Generic;
using System.Globalization;

namespace Applique.LoadTester.Environment.Test.Bindings
{
    public abstract class BindingsTestBase<TReturn> : BindingsTestBase
    {
        protected TReturn ReturnValue;
    }

    public abstract class BindingsTestBase
    {
        protected const string SomeConstant = "SomeConstant";
        protected const string AnotherConstant = "AnotherConstant";
        protected const string SomeString = "SomeString";
        protected const string AnotherString = "AnotherString";
        protected const int SomeInt = 123;
        protected const int AnotherInt = 456;
        protected const decimal SomeDecimal = 123.45M;
        protected const decimal AnotherDecimal = 456.78M;

        protected IDictionary<string, object> Variables = new Dictionary<string, object>();
        protected IDictionary<string, object> OverloadVariables = new Dictionary<string, object>();

        protected IBindings SUT { get; private set; }

        protected void Arrange()
        {
            Given();
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            SUT = CreateBindings(Variables);
        }

        protected static IBindings CreateBindings(IDictionary<string, object> variables)
            => new Environment.Bindings(new BindingVariables(variables));

        protected void ArrangeAndAct()
        {
            Arrange();
            Act();
        }

        protected abstract void Act();

        protected virtual void Given() { }
    }
}