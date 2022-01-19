using System;
using System.Collections.Generic;
using System.Linq;

namespace Applique.LoadTester.Business.Design
{
    public class TestSuite
    {
        public string Name { get; set; }
        public Constant[] Constants { get; set; } = Array.Empty<Constant>();
        public Model[] Models { get; set; } = Array.Empty<Model>();
        public Service[] Services { get; set; }
        public Blob[] Blobs { get; set; }
        public StepTemplate[] StepTemplates { private get; set; }
        public Scenario[] Templates { private get; set; }
        public Scenario[] Scenarios { private get; set; }

        public IEnumerable<Scenario> RunnableScenarios => Scenarios.Where(scenario => !scenario.Disabled);

        public Blob GetBlob(string name)
            => Blobs.SingleOrDefault(t => t.Name == name)
            ?? throw new NotImplementedException($"Blob: {name}");

        public Scenario GetTemplate(string name)
            => Templates.SingleOrDefault(t => t.Name == name)
            ?? throw new NotImplementedException($"Template: {name}");

        public Step GetStepToRun(Step step)
            => step.Template is null ? step : GetStepTemplate(step.Template).Step;

        private StepTemplate GetStepTemplate(string name)
            => StepTemplates.SingleOrDefault(t => t.Name == name)
            ?? throw new NotImplementedException($"StepTemplate: {name}");
    }
}