using Applique.LoadTester.Domain.Assembly;
using System.Collections.Generic;
using System.Linq;

namespace Applique.LoadTester.Logic.Assembly;

public class TestSuiteScenario : ITestSuiteScenario
{
    private readonly TestSuite _testSuite;
    private readonly Scenario _scenario;

    public TestSuiteScenario(TestSuite testSuite, Scenario scenario)
    {
        _testSuite = testSuite;
        _scenario = scenario;
    }

    public IScenario Scenario => _scenario;

    public IEnumerable<IStep> GetStepsToRun()
        => _scenario.Steps.Select(GetStepToRun).ToArray();

    private IStep GetStepToRun(IStep step)
        => step.Template is null
            ? step
            : GetStepToRun(_testSuite.GetStepTemplate(step.Template)).MergeWith(step);
}