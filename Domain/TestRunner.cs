using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Engine;
using Applique.LoadTester.Domain.Environment;
using Applique.LoadTester.Domain.Result;
using Applique.LoadTester.Home;

namespace Applique.LoadTester.Domain
{
    public class TestRunner
    {
        private readonly IFileSystem _fileSystem;
        private readonly IScenarioRunnerFactory _scenarioRunnerFactory;
        private readonly IAssembler _assembler;

        public TestRunner(IFileSystem fileSystem, IScenarioRunnerFactory scenarioRunnerFactory, IAssembler assembler)
        {
            _fileSystem = fileSystem;
            _scenarioRunnerFactory = scenarioRunnerFactory;
            _assembler = assembler;
        }

        public async Task Run(string testSuiteFileName)
        {
            var testSuite = LoadTestSuite(testSuiteFileName);
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

        private void SaveResultToFile(string testName, string[] resultLines)
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

        private ITestSuite LoadTestSuite(string filename)
        {
            try
            {
                return _assembler.ReadTestSuite(filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load testsuite because: '{ex.Message}'. Write basePath as first argument and filename of test suite as second program argument.");
                Console.WriteLine("Press a key to continue.");
                Console.ReadKey();
                throw;
            }
        }

        private async Task<TestSuiteResult> RunTestSuite(ITestSuite testSuite)
        {
            var scenarioRunner = _scenarioRunnerFactory.Create(testSuite);
            var results = new List<IScenarioResult>();
            foreach (var scenario in testSuite.RunnableScenarios)
            {
                Console.WriteLine("--------------------------");
                Console.WriteLine($"Running scenario: {scenario.Name} with {scenario.Instances} instances");
                var result = await scenarioRunner.Run(scenario);
                results.Add(result);
                Console.WriteLine("Scenario " + (result.Success ? "succeeded" : "failed"));
                if (!result.Success)
                    break;
            }
            return new TestSuiteResult(results.OrderByDescending(res => res.Success).ToArray());
        }
    }
}