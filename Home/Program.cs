using Applique.LoadTester.External;
using System.Globalization;
using System.Threading.Tasks;
using Applique.LoadTester.Domain;
using Applique.LoadTester.Runtime.Engine;
using Applique.LoadTester.Assembly;
using Applique.LoadTester.Environment;

namespace Applique.LoadTester.Home
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
            var assembler = new Assembler(fileSystem);
            var bindingsFactory = new BindingsFactory(fileSystem);
            var stepVerifierFactory = new StepVerifierFactory();
            var scenarioRunnerFactory = new ScenarioRunnerFactory(
                fileSystem, 
                restCallerFactory, 
                blobRepositoryFactory, 
                assembler,
                bindingsFactory,
                stepVerifierFactory);
            var testRunner = new TestRunner(fileSystem, scenarioRunnerFactory, assembler);
            await testRunner.Run(testSuiteFileName);
        }
    }
}