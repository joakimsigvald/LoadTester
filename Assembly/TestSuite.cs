using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Domain.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using static Applique.LoadTester.Domain.Service.ConstantFactory;

namespace Applique.LoadTester.Assembly
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

        public IEnumerable<IScenario> RunnableScenarios => Scenarios.Where(scenario => !scenario.Disabled);

        public Blob GetBlob(string name)
            => Blobs.SingleOrDefault(t => t.Name == name)
            ?? throw new NotImplementedException($"Blob: {name}");

        public IScenario GetTemplate(string name)
            => Templates.SingleOrDefault(t => t.Name == name)
            ?? throw new NotImplementedException($"Template: {name}");

        public Step GetStepToRun(Step step)
            => step.Template is null
                ? step
                : MergeSteps(GetStepTemplate(step.Template).Step, step);

        private StepTemplate GetStepTemplate(string name)
            => StepTemplates.SingleOrDefault(t => t.Name == name)
            ?? throw new NotImplementedException($"StepTemplate: {name}");

        private static Step MergeSteps(Step template, Step step)
            => new()
            {
                Args = template.Args,
                Body = step.Body ?? template.Body,
                Constants = template.Constants.Merge(step.Constants),
                BreakOnSuccess = template.BreakOnSuccess,
                Endpoint = step.Endpoint ?? template.Endpoint,
                ExpectedStatusCodes = template.ExpectedStatusCodes,
                Response = step.Response ?? template.Response,
                RetryOnFail = template.RetryOnFail,
                DelayMs = template.DelayMs,
                Times = template.Times,
                Type = template.Type
            };
    }
}