using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Assembly;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;
using System.Collections.Generic;
using System.Linq;

namespace Applique.LoadTester.Logic.Environment;

public class BindingsFactory : IBindingsFactory
{
    private readonly IFileSystem _fileSystem;

    public BindingsFactory(IFileSystem fileSystem) => _fileSystem = fileSystem;

    public IBindings CreateInstanceBindings(ITestSuite testSuite, IScenario scenario, int instanceId)
    {
        var constants = GetConstants(testSuite, scenario.Constants, instanceId);
        return CreateBindings(testSuite, constants);
    }

    public IBindings CreateBindings(ITestSuite testSuite, Constant[] constants)
        => new Bindings(new BindingVariables(CreateVariables(testSuite, constants)));

    private static Constant[] GetConstants(ITestSuite testSuite, Constant[] scenarioConstants, int instanceId)
        => GetInstanceConstants(testSuite, instanceId).Merge(scenarioConstants);

    private static IEnumerable<Constant> GetInstanceConstants(ITestSuite testSuite, int instanceId)
        => testSuite.Constants.Prepend(ConstantFactory.Create("InstanceId", $"{instanceId}"));

    private IDictionary<string, object> CreateVariables(
        ITestSuite suite,
        IEnumerable<Constant> constants)
        => constants.ToDictionary(c => c.Name, c => GetValue(suite, c));

    private object GetValue(ITestSuite testSuite, Constant constant)
        => constant.Type == ConstantType.Seed
        ? GetSeed(testSuite, constant)
        : ConstantExpressions.ValueOf(constant);

    private object GetSeed(ITestSuite testSuite, Constant constant)
    {
        var seed = LoadSeed(testSuite, constant) ?? int.Parse(constant.Value);
        SaveSeed(testSuite, constant, seed + 1);
        return seed;
    }

    private void SaveSeed(ITestSuite testSuite, Constant constant, int seed)
        => _fileSystem.Write(GetSeedPath(testSuite, constant), seed);

    private int? LoadSeed(ITestSuite testSuite, Constant constant)
        => _fileSystem.Exists(GetSeedPath(testSuite, constant))
        ? _fileSystem.Read<int>(global::Applique.LoadTester.Logic.Environment.BindingsFactory.GetSeedPath(testSuite, constant))
        : null;

    private static string GetSeedPath(ITestSuite testSuite, Constant constant)
        => $"{testSuite.Name}_{constant.Name}_Seed";
}