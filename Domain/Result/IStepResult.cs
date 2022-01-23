using Applique.LoadTester.Domain.Design;

namespace Applique.LoadTester.Domain.Result
{
    public interface IStepResult : ITimedResult
    {
        Step Step { get; }
    }
}