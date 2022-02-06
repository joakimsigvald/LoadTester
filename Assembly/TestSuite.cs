using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Domain.Design;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Applique.LoadTester.Logic.Assembly
{
    public class TestSuite : ITestSuite
    {
        public string Name { get; set; }
        public Constant[] Constants { get; set; } = Array.Empty<Constant>();
        public Service[] Services { get; set; }
        public Blob[] Blobs { get; set; }
        public StepTemplate[] StepTemplates { private get; set; }
        public Scenario[] Templates { private get; set; }
        public Scenario[] Scenarios { private get; set; }

        public IEnumerable<IScenario> ScenariosToRun 
            => Scenarios.Where(scenario => !scenario.Disabled)
            .Select(GetScenarioToRun);

        public Blob GetBlob(string name)
            => Blobs.SingleOrDefault(t => t.Name == name)
            ?? throw new NotImplementedException($"Blob: {name}");

        public IStep GetStepTemplate(string name)
            => StepTemplates.SingleOrDefault(t => t.Name == name).Step
            ?? throw new NotImplementedException($"StepTemplate: {name}");

        private Scenario GetScenarioToRun(Scenario scenario)
            => scenario.Template == null
            ? scenario
            : GetScenarioToRun(GetTemplate(scenario.Template)).MergeWith(scenario);

        private Scenario GetTemplate(string name)
            => Templates.SingleOrDefault(t => t.Name == name)
            ?? throw new NotImplementedException($"Template: {name}");
    }
}