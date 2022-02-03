using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Domain.Design;
using System.Linq;
using Xunit;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Assembly.Test.TestSuite
{
    public abstract class WhenGetStepToRun : TestSuiteTestBase<Step>
    {
        protected Step Step;
        protected string TemplateName;
        protected Constant[] StepConstants;

        protected override void Act()
            => ReturnValue = SUT.GetStepToRun(new Step
            {
                Template = TemplateName,
                Constants = StepConstants
            });

        public class GivenTemplate : WhenGetStepToRun
        {
            [Fact]
            public void ThenMergeConstantsFromTemplate()
            {
                TemplateName = "TheTemplate";
                var myValue = "MyValue";
                SuiteConstants = new[]
                {
                    new Constant { Name = SomeConstant, Value = SomeString }
                };
                var templateConstants = new[]
                {
                    new Constant { Name = SomeConstant, Value = AnotherString }
                };
                StepConstants = new[]
                {
                    new Constant { Name = SomeConstant, Value = myValue }
                };
                StepTemplates = new[]
                {
                    new StepTemplate
                    {
                        Name = TemplateName,
                        Step = new Step
                        {
                            Constants = templateConstants
                        }
                    }
                };
                ArrangeAndAct();
                var actual = ReturnValue.Constants.Single(c => c.Name == SomeConstant).Value;
                Assert.Equal(myValue, actual);
            }
        }
    }
}
