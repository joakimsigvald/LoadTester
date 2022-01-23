using Applique.LoadTester.Domain.Design;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Applique.LoadTester.Domain.Environment
{
    public interface IBindings : IEnumerable<Constant>
    {
        string SubstituteVariables(string target);
        bool TrySubstituteVariable(string target, out string value);
        void BindVariables(JObject pattern, JObject source);
        string CreateContent(object body);
    }
}