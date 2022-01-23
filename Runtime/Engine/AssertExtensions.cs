using Applique.LoadTester.Runtime.Environment;
using Applique.LoadTester.Runtime.Result;
using Applique.LoadTester.Design;

namespace Applique.LoadTester.Runtime.Engine
{
    public static class AssertExtensions
    {
        public static AssertResult Apply(this Assert assert, Bindings bindings, object actualValue)
        {
            var value = bindings.SubstituteVariables(assert.Value);
            var res = $"{actualValue}" == value;
            return res ? new AssertResult
            {
                Success = true,
                Message = $"{assert.Name} is {actualValue} as expected"
            }
            : new AssertResult { Message = $"{assert.Name} is {actualValue} but expected {value}" };
        }
    }
}