using System;

namespace Applique.LoadTester.Domain.Engine
{
    public class RunFailed : Exception
    {
        public RunFailed(string message) : base(message)
        {
        }
    }
}