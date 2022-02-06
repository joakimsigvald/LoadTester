using Applique.LoadTester.Domain.Design;
using System.Linq;
using Xunit;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Logic.Assembly.Test.TestSuite
{
    public abstract class WhenGetScenarioToRun : TestSuiteTestBase<IScenario>
    {
        protected Scenario Scenario = new();

        protected override void Act() => ReturnValue = SUT.GetScenarioToRun(Scenario);

        public class GivenNoTemplate : WhenGetScenarioToRun
        {
            [Fact]
            public void ThenReturnScenarioFromScript()
            {
                ArrangeAndAct();
                Assert.Same(Scenario, ReturnValue);
            }
        }

        public abstract class GivenTemplate : WhenGetScenarioToRun
        {
            protected Scenario TemplateScenario = new();

            protected override void Given()
            {
                Scenario.Template = "TheTemplate";
                Templates = new[]
                {
                    new Scenario
                    {
                        Name = Scenario.Template,
                    }
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
                var actual = ReturnValue.Constants.Single(c => c.Name == SomeConstant).Value;
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
                Assert.Equal(expected, ReturnValue.Instances);
            }
        }

        public class GivenName : GivenTemplate
        {
            [Fact]
            public void ThenGetNameFromScenario()
            {
                Scenario.Name = "MyName";
                ArrangeAndAct();
                Assert.Equal(Scenario.Name, ReturnValue.Name);
            }
        }
    }
}