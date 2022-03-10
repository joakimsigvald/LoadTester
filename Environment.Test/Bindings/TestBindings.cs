using Applique.LoadTester.Core.Service;
using Applique.WhenGivenThen.Core;
using System.Collections.Generic;

namespace Applique.LoadTester.Logic.Environment.Test.Bindings
{
    public abstract class TestBindings<TReturn> : TestSubject<IBindings, TReturn>
    {
        protected IDictionary<string, object> Variables = new Dictionary<string, object>();
        protected IDictionary<string, object> OverloadVariables = new Dictionary<string, object>();

        protected override IBindings CreateSUT()
        {
            var bindings = CreateBindings(Variables);
            bindings.OverloadWith(CreateBindings(OverloadVariables));
            return bindings;
        }

        protected static IBindings CreateBindings(IDictionary<string, object> variables)
            => new Environment.Bindings(new BindingVariables(variables));
    }
}