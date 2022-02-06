using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Domain.Design;
using System;
using System.Linq;

namespace Applique.LoadTester.Logic.Assembly
{
    public class Scenario : IScenario
    {
        public string Name { get; set; }
        public string Template { get; set; }
        public bool Disabled { get; set; }
        public int Instances { get; set; } = 1;
        public string[] Load { get; set; } = Array.Empty<string>();
        public string[] Persist { get; set; } = Array.Empty<string>();
        public Constant[] Constants { get; set; } = Array.Empty<Constant>();
        public Step[] Steps { get; set; } = Array.Empty<Step>();

        public IStep[] GetStepsToRun(ITestSuite testSuite)
        {
            return Steps.Select(GetStepToRun).ToArray();

            IStep GetStepToRun(IStep step)
                => step.Template is null
                    ? step
                    : GetStepToRun(testSuite.GetStepTemplate(step.Template)).MergeWith(step);
        }

        public Scenario MergeWith(Scenario other)
            => new ()
            {
                Constants = Constants.Merge(other.Constants),
                Instances = other.Instances,
                Load = other.Load,
                Name = other.Name,
                Persist = other.Persist,
                Steps = Steps.Concat(other.Steps).ToArray()
            };
    }
}