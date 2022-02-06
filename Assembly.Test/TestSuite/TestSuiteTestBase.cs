using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Test;
using System.Linq;

namespace Applique.LoadTester.Logic.Assembly.Test.TestSuite
{
    public abstract class TestSuiteTestBase : TestBase<Assembly.TestSuite>
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
    }

    public abstract class TestSuiteTestBase<TReturn> : TestSuiteTestBase
    {
        protected TReturn ReturnValue;
    }
}