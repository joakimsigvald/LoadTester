using Applique.LoadTester.Core.Design;

namespace Applique.LoadTester.Domain.Assembly
{
    public interface IScenario : IScenarioMetadata
    {
        string Template { get; }
        string[] Load { get; }
        string[] Persist { get; }
        Constant[] Constants { get; }
    }
}