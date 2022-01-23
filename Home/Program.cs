using Applique.LoadTester.External;
using System.Globalization;
using System.Threading.Tasks;
using Applique.LoadTester.Domain.Environment;
using Applique.LoadTester.Domain;
using Applique.LoadTester.Runtime.Engine;

namespace Applique.LoadTester.Home
{
    class Program
    {
        private static IFileSystem _fileSystem;
        private static IRestCallerFactory _restCallerFactory;
        private static IBlobRepositoryFactory _blobRepositoryFactory;

        static async Task Main(string[] args)
        {
            var basePath = args[0];
            var testSuiteFileName = args[1];
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            _fileSystem = new FileSystem(basePath);
            _restCallerFactory = new RestCallerFactory();
            _blobRepositoryFactory = new BlobRepositoryFactory();
            var scenarioRunnerFactory = new ScenarioRunnerFactory(_fileSystem, _restCallerFactory, _blobRepositoryFactory);
            var testRunner = new TestRunner(_fileSystem, scenarioRunnerFactory);
            await testRunner.Run(testSuiteFileName);
        }
    }
}