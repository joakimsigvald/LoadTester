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

        protected override void Act()
        {
            ReturnValue = SUT.GetStepToRun(Step);
        }

        public class GivenTemplate : WhenGetStepToRun
        {
            [Fact]
            public void ThenMergeConstantsFromTemplate()
            {
                var templateName = "TheTemplate";
                var myValue = "MyValue";
                SuiteConstants = new[]
                {
                    new Constant { Name = SomeConstant, Value = SomeString }
                };
                var templateConstants = new[]
                {
                    new Constant { Name = SomeConstant, Value = AnotherString }
                };
                var stepConstants = new[]
                {
                    new Constant { Name = SomeConstant, Value = myValue }
                };
                StepTemplates = new[]
                {
                    new StepTemplate
                    {
                        Name = templateName,
                        Step = new Step
                        {
                            Constants = templateConstants
                        }
                    }
                };
                Step = new Step
                {
                    Template = templateName,
                    Constants = stepConstants
                };
                ArrangeAndAct();
                var actual = ReturnValue.Constants.Single(c => c.Name == SomeConstant).Value;
                Assert.Equal(myValue, actual);
            }
        }
    }
}
