using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Service;

namespace Applique.LoadTester.Core.Result
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