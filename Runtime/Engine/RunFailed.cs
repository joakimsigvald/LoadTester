using System;

namespace Applique.LoadTester.Runtime.Engine
{
    public class RunFailed : Exception
    {
        public RunFailed(string message) : base(message)
        {
        }
    }
}