using Applique.LoadTester.Runtime.Engine;

namespace Applique.LoadTester.Runtime.Environment
{
    public class BindingFailed : RunFailed
    {
        public BindingFailed(string property, string message) : base($"{property}: {message}")
        {
        }
    }
}