namespace Applique.LoadTester.Business.Runtime.Exceptions
{
    public class VerificationFailed : RunFailed
    {
        public VerificationFailed(string property, string message) : base($"{property}: {message}")
        {
        }
    }
}