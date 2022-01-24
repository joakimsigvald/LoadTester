using Applique.LoadTester.Domain.Design;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Applique.LoadTester.Domain.Environment
{
    public enum Constraint { None, Mandatory }

    public interface IBindings : IEnumerable<Constant>
    {
        object Get(string name);
        string SubstituteVariables(string target);
        void VerifyValue(string prefix, JProperty pp, string actualValue);
        void BindResponse(JToken pattern, JToken responseToken);
        string CreateContent(object body);
        void MergeWith(IBindings bindings);
        IEnumerable<KeyValuePair<string, object>> Variables { get; }
    }
}