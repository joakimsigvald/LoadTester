using Applique.LoadTester.Core.Design;

namespace Applique.LoadTester.Domain.Design
{
    public interface IScenario : IScenarioMetadata
    {
        string Template { get; }
        string[] Load { get; }
        string[] Persist { get; }
        Constant[] Constants { get; }
        Step[] Steps { get; }
        Assert[] Asserts { get; }
        IScenario MergeWith(IScenario scenario);
    }
}