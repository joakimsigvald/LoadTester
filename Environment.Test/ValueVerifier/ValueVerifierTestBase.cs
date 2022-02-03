using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Service;
using Applique.LoadTester.Environment;
using Applique.LoadTester.Test;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Logic.Environment.Test.ValueVerifier
{
    public abstract class ValueVerifierTestBase : TestBase<IValueVerifier>
    {
        protected const string _propertyName = "SomeProperty";

        protected string Prefix = string.Empty;
        protected JProperty Template;
        protected object TemplateValue;
        protected string Value = SomeString;

        protected override void Given() => Template = new JProperty(_propertyName, TemplateValue);
        protected override void Act() => SUT.VerifyValue(Prefix, Template, Value);

        protected IDictionary<string, object> Variables = new Dictionary<string, object>();

        protected override IValueVerifier CreateSUT()
        {
            var bindings = CreateBindings(Variables);
            return new Environment.ValueVerifier(bindings);
        }

        protected static IBindings CreateBindings(IDictionary<string, object> variables)
            => new Logic.Environment.Bindings(new BindingVariables(variables));
    }
}