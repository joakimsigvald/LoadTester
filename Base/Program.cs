using Applique.LoadTester.External;
using System.Globalization;
using System.Threading.Tasks;
using Applique.LoadTester.Front;
using Applique.LoadTester.Logic.Environment;
using Applique.LoadTester.Logic.Assembly;
using Applique.LoadTester.Logic.Runtime.Engine;

namespace Applique.LoadTester.Base
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            var basePath = args[0];
            var testSuiteFileName = args[1];
            await CreateTestRunner(basePath).Run(testSuiteFileName);
        }

        private static TestRunner CreateTestRunner(string basePath)
        {
            var fileSystem = new FileSystem(basePath);
            return new TestRunner(fileSystem, CreateAssembler(fileSystem));
        }

        private static Assembler CreateAssembler(FileSystem fileSystem)
            => new(fileSystem, new TestSuiteRunnerFactory(CreateScenarioRunnerFactory(fileSystem)));

        private static ScenarioRunnerFactory CreateScenarioRunnerFactory(FileSystem fileSystem)
        {
            BindingsFactory bindingsFactory = new(fileSystem);
            return new ScenarioRunnerFactory(
                bindingsFactory,
                CreateBindingsRepositoryFactory(fileSystem, bindingsFactory),
                CreateStepInstantiatorFactory(bindingsFactory));
        }

        private static BindingsRepositoryFactory CreateBindingsRepositoryFactory(
            FileSystem fileSystem, BindingsFactory bindingsFactory)
        {
            Loader loader = new(fileSystem);
            return new(fileSystem, loader, bindingsFactory);
        }

        private static StepInstantiatorFactory CreateStepInstantiatorFactory(BindingsFactory bindingsFactory)
        {
            RestCallerFactory restCallerFactory = new();
            BlobRepositoryFactory blobRepositoryFactory = new();
            StepVerifierFactory stepVerifierFactory = new();
            return new(restCallerFactory, blobRepositoryFactory, bindingsFactory, stepVerifierFactory);
        }
    }
}