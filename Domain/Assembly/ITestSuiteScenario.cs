using System.Collections.Generic;

namespace Applique.LoadTester.Domain.Assembly;

public interface ITestSuiteScenario
{
    IScenario Scenario { get; }

    IEnumerable<IStep> GetStepsToRun();
}