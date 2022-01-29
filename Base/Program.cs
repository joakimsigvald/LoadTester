using Applique.LoadTester.External;
using System.Globalization;
using System.Threading.Tasks;
using Applique.LoadTester.Runtime.Engine;
using Applique.LoadTester.Assembly;
using Applique.LoadTester.Environment;
using Applique.LoadTester.Front;

namespace Applique.LoadTester.Base
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var basePath = args[0];
            var testSuiteFileName = args[1];
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            var fileSystem = new FileSystem(basePath);
            var restCallerFactory = new RestCallerFactory();
            var blobRepositoryFactory = new BlobRepositoryFactory();
            var loader = new Loader(fileSystem);
            var bindingsFactory = new BindingsFactory(fileSystem);
            var stepVerifierFactory = new StepVerifierFactory();
            var scenarioRunnerFactory = new ScenarioRunnerFactory(
                fileSystem,
                restCallerFactory,
                blobRepositoryFactory,
                loader,
                bindingsFactory,
                stepVerifierFactory);
            var assembler = new Assembler(fileSystem, scenarioRunnerFactory);
            var testRunner = new TestRunner(fileSystem, assembler);
            await testRunner.Run(testSuiteFileName);
        }
    }
}