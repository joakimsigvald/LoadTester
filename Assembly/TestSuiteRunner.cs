﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Applique.LoadTester.Core.Result;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;

namespace Applique.LoadTester.Assembly
{
    public class TestSuiteRunner : ITestSuiteRunner
    {
        private readonly IScenarioRunnerFactory _scenarioRunnerFactory;
        private readonly ITestSuite _testSuite;

        public TestSuiteRunner(IScenarioRunnerFactory scenarioRunnerFactory, ITestSuite testSuite)
        {
            _scenarioRunnerFactory = scenarioRunnerFactory;
            _testSuite = testSuite;
        }

        public string TestSuiteName => _testSuite.Name;

        public async Task<TestSuiteResult> Run()
        {
            var scenarioRunner = _scenarioRunnerFactory.Create(_testSuite);
            var results = new List<IScenarioResult>();
            foreach (var scenario in _testSuite.RunnableScenarios)
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