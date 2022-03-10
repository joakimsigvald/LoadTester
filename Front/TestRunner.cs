using System;
using System.Threading.Tasks;
using Applique.LoadTester.Core.Result;
using Applique.LoadTester.Core.Service;

namespace Applique.LoadTester.Front;

public class TestRunner
{
    private readonly IFileSystem _fileSystem;
    private readonly IAssembler _assembler;

    public TestRunner(IFileSystem fileSystem, IAssembler assembler)
    {
        _fileSystem = fileSystem;
        _assembler = assembler;
    }

    public async Task Run(string testSuiteFileName)
    {
        var runner = LoadTestSuite(testSuiteFileName);
        do
        {
            Console.WriteLine("Running test suite: " + runner.TestSuiteName);
            var result = await runner.Run();
            var resultLines = OutputResult(runner.TestSuiteName, result);
            if (ShouldSaveResult())
                SaveResultToFile(runner.TestSuiteName, resultLines);
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

    private ITestSuiteRunner LoadTestSuite(string filename)
    {
        try
        {
            return _assembler.AssembleTestSuite(filename);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load testsuite because: '{ex.Message}'. Write basePath as first argument and filename of test suite as second program argument.");
            Console.WriteLine("Press a key to continue.");
            Console.ReadKey();
            throw;
        }
    }
}