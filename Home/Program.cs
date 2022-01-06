using Applique.LoadTester.Business.Design;
using Applique.LoadTester.Business.Result;
using Applique.LoadTester.Business.Runtime;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Applique.LoadTester.Home
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var testSuite = LoadTestSuite(args.FirstOrDefault());
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
            while (File.Exists(GetFileName(testName, i)))
                i++;
            File.WriteAllLines(GetFileName(testName, i), resultLines);
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
                var json = File.ReadAllText(filename);
                return JsonConvert.DeserializeObject<TestSuite>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load testsuite because: '{ex.Message}'. Write filename of test suite as first program argument.");
                Console.WriteLine("Press a key to continue.");
                Console.ReadKey();
                throw;
            }
        }

        private static Task<TestSuiteResult> RunTestSuite(TestSuite testSuite)
            => new TestSuiteRunner(testSuite).Run();
    }
}
