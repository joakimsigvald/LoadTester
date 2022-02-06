using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Test;
using System.Linq;

namespace Applique.LoadTester.Logic.Assembly.Test.Scenario
{
    public abstract class ScenarioTestBase : TestBase<Assembly.Scenario>
    {
        protected Step Step = new();
        protected StepTemplate[] StepTemplates;
        protected Assembly.Scenario[] Templates;

        protected override Assembly.Scenario CreateSUT()
            => new()
            {
                Steps = new[] { Step }
            };

        protected static Constant[] CreateConstants(params (string name, string value)[] namedValues)
            => namedValues.Select(nv => new Constant { Name = nv.name, Value = nv.value }).ToArray();
    }

    public abstract class ScenarioTestBase<TReturn> : ScenarioTestBase
    {
        protected TReturn ReturnValue;
    }
}