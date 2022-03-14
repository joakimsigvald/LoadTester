using Applique.LoadTester.Domain.Assembly;
using Applique.WhenGivenThen.Core;
using Applique.LoadTester.Core.Design;
using System.Linq;
using Xunit;

namespace Applique.LoadTester.Logic.Assembly.Test.TestSuiteScenario;

public abstract class WhenGetStepsToRun : TestSubject<Assembly.TestSuiteScenario, IStep>
{
    protected Assembly.Step Step = new();
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

    public class GivenStepWithoutTemplate : WhenGetStepsToRun
    {
        [Fact]
        public void ThenReturnStepFromScript()
        {
            ArrangeAndAct();
            Assert.Same(Step, Result);
        }
    }

    public abstract class GivenStepWithTemplate : WhenGetStepsToRun
    {
        protected override void Given()
        {
            Step.Template = "TheTemplate";
            StepTemplates = new[]
            {
                new StepTemplate
                {
                    Name = Step.Template,
                    Step = MockOf<Assembly.Step>()
                }
            };
        }

        [Fact]
        public void ThenMergeTemplateWithStep()
        {
            ArrangeAndAct();
            Mocked<Assembly.Step>().Verify(template => template.MergeWith(Step));
        }
    }

    public abstract class GivenTemplateWithTemplate : WhenGetStepsToRun
    {
        private StepTemplate _innerTemplate, _outerTemplate;

        protected override void Given()
        {
            _innerTemplate = new StepTemplate
            {
                Name = "inner",
                Step = MockOf<Assembly.Step>()
            };
            _outerTemplate = new StepTemplate
            {
                Name = "outer",
                Step = new Assembly.Step { Template = _innerTemplate.Name }
            };
            Step.Template = _outerTemplate.Name;
            StepTemplates = new[] { _innerTemplate, _outerTemplate};
        }

        [Fact]
        public void ThenTemplatesAreMerged()
        {
            ArrangeAndAct();
            Mocked<Assembly.Step>().Verify(template => template.MergeWith(_outerTemplate.Step));
        }
    }
}