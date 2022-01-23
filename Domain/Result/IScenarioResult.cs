using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Environment;

namespace Applique.LoadTester.Domain.Result
{
    public interface IScenarioResult : ITimedResult
    {
        bool Success { get; }
        IStepResult[] StepResults { get; }
        IBindings Bindings { get; }
        IScenario Scenario { get; }
        string Error { get; }
    }
}