using System.Collections.Generic;

namespace Applique.LoadTester.Domain.Design
{
    public interface ITestSuite
    {
        string Name { get; }
        Service[] Services { get; }
        Model[] Models { get; }
        IEnumerable<IScenario> RunnableScenarios { get; }
        IScenario GetTemplate(string name);
        Step GetStepToRun(Step step);
        Blob GetBlob(string name);
        IEnumerable<Constant> GetInstanceConstants(int instanceId);
    }
}