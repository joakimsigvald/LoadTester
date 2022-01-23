using System;
using System.Linq;

namespace Applique.LoadTester.Design
{
    public class Scenario
    {
        public string Name { get; set; }
        public string Template { get; set; }
        public bool Disabled { get; set; }
        public int Instances { get; set; } = 1;
        public string[] Load { get; set; } = Array.Empty<string>();
        public string[] Persist { get; set; } = Array.Empty<string>();
        public Constant[] Constants { get; set; } = Array.Empty<Constant>();
        public Step[] Steps { get; set; } = Array.Empty<Step>();
        public Assert[] Asserts { get; set; } = Array.Empty<Assert>();

        public Scenario MergeWith(Scenario scenario)
            => new()
            {
                Asserts = Asserts.Concat(scenario.Asserts).ToArray(),
                Constants = Constants.Concat(scenario.Constants).ToArray(),
                Instances = scenario.Instances,
                Load = scenario.Load,
                Name = scenario.Name,
                Persist = scenario.Persist,
                Steps = Steps.Concat(scenario.Steps).ToArray()
            };
    }
}