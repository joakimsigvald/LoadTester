using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Domain.Assembly;
using Applique.WhenGivenThen.Core;
using System.Linq;
using Xunit;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Logic.Assembly.Test.TestSuite
{
    public abstract class WhenGetScenariosToRun : TestSubject<Assembly.TestSuite, IScenario>
    {
        protected StepTemplate[] StepTemplates;
        protected Scenario[] Templates;
        protected Scenario Scenario = new();

        protected override Assembly.TestSuite CreateSUT()
            => new()
            {
                StepTemplates = StepTemplates,
                Templates = Templates,
                Scenarios = new[] { Scenario }
            };

        protected static Constant[] CreateConstants(params (string name, string value)[] namedValues)
            => namedValues.Select(nv => new Constant { Name = nv.name, Value = nv.value }).ToArray();

        protected override void Act() => CollectResult(() => SUT.ScenarioWrappers.Single().Scenario);

        public class GivenNoTemplate : WhenGetScenariosToRun
        {
            [Fact]
            public void ThenReturnScenarioFromScript()
            {
                ArrangeAndAct();
                Assert.Same(Scenario, Result);
            }
        }

        public abstract class GivenTemplate : WhenGetScenariosToRun
        {
            protected Scenario TemplateScenario = new();

            protected override void Given()
            {
                Scenario.Template = "TheTemplate";
                TemplateScenario.Name = Scenario.Template;
                Templates = new[]
                {
                    TemplateScenario
                };
                base.Given();
            }
        }

        public class GivenScenarioAndTemplateHasConstants : GivenTemplate
        {
            [Fact]
            public void ThenMergeConstants()
            {
                TemplateScenario.Constants = CreateConstants((SomeConstant, AnotherString));
                Scenario.Constants = CreateConstants((SomeConstant, SomeString));
                ArrangeAndAct();
                var actual = Result.Constants.Single(c => c.Name == SomeConstant).Value;
                Assert.Equal(SomeString, actual);
            }
        }

        public class GivenInstances : GivenTemplate
        {
            [Theory]
            [InlineData(1, 1, 1)]
            [InlineData(1, 2, 2)]
            [InlineData(2, 1, 1)]
            [InlineData(2, 2, 2)]
            public void ThenGetInstansesFromScenario(int fromTemplate, int fromStep, int expected)
            {
                TemplateScenario.Instances = fromTemplate;
                Scenario.Instances = fromStep;
                ArrangeAndAct();
                Assert.Equal(expected, Result.Instances);
            }
        }

        public class GivenName : GivenTemplate
        {
            [Fact]
            public void ThenGetNameFromScenario()
            {
                Scenario.Name = "MyName";
                ArrangeAndAct();
                Assert.Equal(Scenario.Name, Result.Name);
            }
        }

        public class GivenLoad : GivenTemplate
        {
            [Fact]
            public void ThenGetLoadFromScenario()
            {
                Scenario.Load = new[] { "A" };
                TemplateScenario.Load = new[] { "B" };
                ArrangeAndAct();
                Assert.Equal(Scenario.Load, Result.Load);
            }
        }

        public class GivenPersist : GivenTemplate
        {
            [Fact]
            public void ThenGetPersistFromScenario()
            {
                Scenario.Persist = new[] { "A" };
                TemplateScenario.Persist = new[] { "B" };
                ArrangeAndAct();
                Assert.Equal(Scenario.Persist, Result.Persist);
            }
        }

        public class GivenSteps : GivenTemplate
        {
            [Fact]
            public void ThenGetTemplateStepsFollowedByScenarioStepsPersistFromScenario()
            {
                var steps = new[] { "A", "B", "C", "D" }.Select(v => new Step { Endpoint = v }).ToArray();
                TemplateScenario.Steps = steps[..2];
                Scenario.Steps = steps[2..];
                ArrangeAndAct();
                Assert.Equal(steps, ((Scenario)Result).Steps);
            }
        }
    }
}