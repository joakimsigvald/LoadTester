using System;

namespace Applique.LoadTester.Core.Result
{
    public interface ITimedResult
    {
        TimeSpan Min { get; }
        TimeSpan Max { get; }
        TimeSpan Mean { get; }
        TimeSpan Q75 { get; }
        TimeSpan Q90 { get; }
    }
}