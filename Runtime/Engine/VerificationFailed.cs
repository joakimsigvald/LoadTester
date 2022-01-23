namespace Applique.LoadTester.Runtime.Engine
{
    public class VerificationFailed : RunFailed
    {
        public VerificationFailed(string property, string message) : base($"{property}: {message}")
        {
        }
    }
}