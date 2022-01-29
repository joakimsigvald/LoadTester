using Applique.LoadTester.Domain;

namespace Applique.LoadTester.Runtime.Environment
{
    internal class BindingFailed : RunFailed
    {
        public BindingFailed(string property, string message) : base($"{property}: {message}")
        {
        }
    }
}