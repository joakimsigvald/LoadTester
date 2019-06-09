using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LoadTester
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var testSuite = LoadTestSuite(args.FirstOrDefault());
            Console.WriteLine("Running test suite: " + testSuite.Name);
            var result = await RunTestSuite(testSuite);
            TestResultPresenter.PresentResult(testSuite.Name, result);
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
