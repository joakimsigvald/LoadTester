using Applique.LoadTester.External;
using System.Globalization;
using Applique.LoadTester.Front;
using Applique.LoadTester.Logic.Environment;
using Applique.LoadTester.Logic.Assembly;
using Applique.LoadTester.Logic.Runtime.Engine;
using Applique.LoadTester.Base;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
var basePath = args[0];
var testSuiteFileName = args[1];
await CreateTestRunner(basePath).Run(testSuiteFileName);

static TestRunner CreateTestRunner(string basePath)
{
    FileSystem fileSystem = new (basePath);
    return new TestRunner(fileSystem, CreateAssembler(fileSystem));
}

static Assembler CreateAssembler(FileSystem fileSystem)
    => new(fileSystem, new TestSuiteRunnerFactory(CreateScenarioRunnerFactory(fileSystem)));

static ScenarioRunnerFactory CreateScenarioRunnerFactory(FileSystem fileSystem)
{
    BindingsFactory bindingsFactory = new(fileSystem);
    return new ScenarioRunnerFactory(
        CreateBindingsRepositoryFactory(fileSystem, bindingsFactory),
        CreateScenarioInstantiatorFactory(bindingsFactory));
}

static BindingsRepositoryFactory CreateBindingsRepositoryFactory(
    FileSystem fileSystem, BindingsFactory bindingsFactory)
    => new(fileSystem, new Loader(fileSystem), bindingsFactory);

static ScenarioInstantiatorFactory CreateScenarioInstantiatorFactory(BindingsFactory bindingsFactory)
    => new(bindingsFactory, CreateStepInstantiatorFactory(bindingsFactory));

static StepInstantiatorFactory CreateStepInstantiatorFactory(BindingsFactory bindingsFactory)
{
    RestCallerFactory restCallerFactory = new();
    BlobRepositoryFactory blobRepositoryFactory = new();
    StepVerifierFactory stepVerifierFactory = new();
    return new(restCallerFactory, blobRepositoryFactory, bindingsFactory, stepVerifierFactory);
}