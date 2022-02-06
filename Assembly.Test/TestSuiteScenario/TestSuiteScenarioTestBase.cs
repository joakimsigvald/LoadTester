using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Test;
using System.Linq;

namespace Applique.LoadTester.Logic.Assembly.Test.TestSuiteScenario
{
    public abstract class TestSuiteScenarioTestBase : TestBase<Assembly.TestSuiteScenario>
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
    }

    public abstract class TestSuiteScenarioTestBase<TReturn> : TestSuiteScenarioTestBase
    {
        protected TReturn ReturnValue;
    }
}