using Applique.LoadTester.Business.Design;
using Applique.LoadTester.Business.External;
using Applique.LoadTester.Business.Result;
using Applique.LoadTester.Business.Runtime;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Applique.LoadTester.Home
{
    class Program
    {
        private static IFileSystem _fileSystem;

        static async Task Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            _fileSystem = new FileSystem(args.FirstOrDefault());
            var testSuite = LoadTestSuite(args.Skip(1).FirstOrDefault());
            do
            {
                Console.WriteLine("Running test suite: " + testSuite.Name);
                var result = await RunTestSuite(testSuite);
                var resultLines = OutputResult(testSuite.Name, result);
                if (ShouldSaveResult())
                    SaveResultToFile(testSuite.Name, resultLines);
            }
            while (ShouldRunAgain());
        }

        private static bool ShouldSaveResult() => Interactor.Ask("Output result to text-file");

        private static bool ShouldRunAgain() => Interactor.Ask("Run again");

        private static void SaveResultToFile(string testName, string[] resultLines)
        {
            int i = 1;
            while (_fileSystem.Exists(GetFileName(testName, i)))
                i++;
            _fileSystem.WriteLines(GetFileName(testName, i), resultLines);
        }

        private static string GetFileName(string name, int n) => $"{name}_{n}.txt";

        private static string[] OutputResult(string name, TestSuiteResult result)
        {
            var resultLines = TestResultPresenter.PresentResult(name, result);
            foreach (var line in resultLines)
                Console.WriteLine(line);
            return resultLines;
        }

        private static TestSuite LoadTestSuite(string filename)
        {
            try
            {
                return _fileSystem.Read<TestSuite>(filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load testsuite because: '{ex.Message}'. Write basePath as first argument and filename of test suite as second program argument.");
                Console.WriteLine("Press a key to continue.");
                Console.ReadKey();
                throw;
            }
        }

        private static Task<TestSuiteResult> RunTestSuite(TestSuite testSuite)
            => new TestSuiteRunner(_fileSystem, testSuite).Run();
    }
}
