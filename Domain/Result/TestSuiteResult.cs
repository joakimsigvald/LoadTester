﻿using System.Linq;

namespace Applique.LoadTester.Domain.Result
{
    public class TestSuiteResult
    {
        public IScenarioResult[] ScenarioResults { get; set; }

        public TestSuiteResult(IScenarioResult[] scenarioResults) => ScenarioResults = scenarioResults;

        public bool Success => ScenarioResults.All(sr => sr.Success);
    }
}