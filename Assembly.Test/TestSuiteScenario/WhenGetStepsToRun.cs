using Applique.LoadTester.Domain.Assembly;
using Applique.WhenGivenThen.Core;
using Applique.LoadTester.Core.Design;
using System.Linq;
using Xunit;

namespace Applique.LoadTester.Logic.Assembly.Test.TestSuiteScenario;

public abstract class WhenGetStepsToRun : TestSubject<Assembly.TestSuiteScenario, IStep>
{
    protected Step Step = new();
    protected StepTemplate[] StepTemplates;
    protected Scenario[] Templates;

    private Assembly.TestSuite TestSuite => new()
    {
        StepTemplates = StepTemplates,
        Templates = Templates
    };

    private Scenario Scenario => new()
    {
        Steps = new[] { Step }
    };

    protected override Assembly.TestSuiteScenario CreateSUT() => new(TestSuite, Scenario);

    protected static Constant[] CreateConstants(params (string name, string value)[] namedValues)
        => namedValues.Select(nv => new Constant { Name = nv.name, Value = nv.value }).ToArray();

    protected override void Act() => CollectResult(() => SUT.GetStepsToRun().Single());

    public class GivenNoTemplate : WhenGetStepsToRun
    {
        [Fact]
        public void ThenReturnStepFromScript()
        {
            ArrangeAndAct();
            Assert.Same(Step, Result);
        }
    }
}