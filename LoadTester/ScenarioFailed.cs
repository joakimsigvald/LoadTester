using System;

namespace LoadTester
{
    public class RunFailed : Exception
    {
        public RunFailed(string message) : base(message)
        {
        }
    }

    public class VerificationFailed : RunFailed
    {
        public VerificationFailed(string message) : base(message)
        {
        }
    }
}