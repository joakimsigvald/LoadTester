using Applique.LoadTester.Core.Design;

namespace Applique.LoadTester.Core.Result
{
    public interface IStepResult : ITimedResult
    {
        Step Step { get; }
    }
}