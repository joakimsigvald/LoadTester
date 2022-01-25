using System.Collections.Generic;
using System.Globalization;

namespace Applique.LoadTester.Environment.Test.Bindings
{
    public abstract class BindingsTestBase
    {
        protected const string ConstantName = "SomeConstant";
        protected const string SomeString = "SomeString";
        protected const string SomeOtherString = "SomeOtherString";
        protected const int SomeInt = 123;
        protected const int SomeOtherInt = 456;
        protected const decimal SomeDecimal = 123.45M;
        
        protected IDictionary<string, object> Variables = new Dictionary<string, object>();

        protected Environment.Bindings Target { get; private set; }

        protected void Setup()
        {
            Arrange();
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Target = new Environment.Bindings(new BindingVariables(Variables));
        }

        protected virtual void Arrange() { }
    }
}