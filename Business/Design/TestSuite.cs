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
        public Scenario[] Templates { private get; set; }
        public Scenario[] Scenarios { private get; set; }

        public IEnumerable<Scenario> RunnableScenarios => Scenarios.Where(scenario => !scenario.Disabled);

        public Scenario GetTemplate(string name)
            => Templates.SingleOrDefault(t => t.Name == name)
            ?? throw new NotImplementedException($"Template: {name}");
    }
}