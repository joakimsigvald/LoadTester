using Applique.LoadTester.Domain;

namespace Applique.LoadTester.Logic.Environment
{
    internal class BindingFailed : RunFailed
    {
        public BindingFailed(string property, string message) : base($"{property}: {message}")
        {
        }
    }
}