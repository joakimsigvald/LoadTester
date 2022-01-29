using System;

namespace Applique.LoadTester.Domain
{
    public class RunFailed : Exception
    {
        public RunFailed(string message) : base(message)
        {
        }
    }
}