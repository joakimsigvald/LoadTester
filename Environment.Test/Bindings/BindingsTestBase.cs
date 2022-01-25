using System.Collections.Generic;
using System.Globalization;

namespace Applique.LoadTester.Environment.Test.Bindings
{
    public abstract class BindingsTestBase
    {
        protected const string ConstantName = "SomeConstant";
        protected const string SomeString = "SomeString";
        protected const int SomeInt = 123;

        protected IDictionary<string, object> Variables = new Dictionary<string, object>();

        protected Environment.Bindings Target { get; private set; }

        protected void Setup()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Target = new Environment.Bindings(new BindingVariables(Variables));
            Arrange();
        }

        protected virtual void Arrange() { }
    }
}