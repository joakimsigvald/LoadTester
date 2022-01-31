using Applique.LoadTester.Core.Design;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Applique.LoadTester.Core.Service
{
    public enum Constraint { None, Mandatory }

    public interface IBindings : IEnumerable<Constant>
    {
        object Get(string name);
        bool TryGet(string name, out object val);
        string SubstituteVariables(string target);
        void BindResponse(JToken pattern, JToken responseToken);
        string CreateContent(object body);
        void MergeWith(IBindings bindings);
        void OverloadWith(IBindings bindings);
        IEnumerable<KeyValuePair<string, object>> Variables { get; }
    }
}