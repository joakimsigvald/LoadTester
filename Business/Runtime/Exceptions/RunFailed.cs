using System;

namespace Applique.LoadTester.Business.Runtime.Exceptions
{
    public class RunFailed : Exception
    {
        public RunFailed(string message) : base(message)
        {
        }
    }
}