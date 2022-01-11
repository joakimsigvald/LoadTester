namespace Applique.LoadTester.Business.Runtime.Exceptions
{
    public class BindingFailed : RunFailed
    {
        public BindingFailed(string property, string message) : base($"{property}: {message}")
        {
        }
    }
}