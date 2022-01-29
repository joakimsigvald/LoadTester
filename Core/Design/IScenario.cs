namespace Applique.LoadTester.Core.Design
{
    public interface IScenario
    {
        string Name { get; }
        string Template { get; }
        int Instances { get; }
        string[] Load { get; }
        string[] Persist { get; }
        Constant[] Constants { get; }
        Step[] Steps { get; }
        Assert[] Asserts { get; }
        IScenario MergeWith(IScenario scenario);
    }
}