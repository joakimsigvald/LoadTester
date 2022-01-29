using Applique.LoadTester.Domain;

namespace Applique.LoadTester.Environment
{
    public class VerificationFailed : RunFailed
    {
        public VerificationFailed(string property, string message) : base($"{property}: {message}")
        {
        }
    }
}