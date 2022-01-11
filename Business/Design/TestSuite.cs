using System;
using System.Collections.Generic;
using System.Linq;

namespace Applique.LoadTester.Business.Design
{
    public class TestSuite
    {
        public string Name { get; set; }
        public Constant[] Constants { get; set; } = Array.Empty<Constant>();
        public Service[] Services { get; set; }
        public Scenario[] Scenarios { private get; set; }

        public IEnumerable<Scenario> RunnableScenarios => Scenarios.Where(scenario => !scenario.Disabled);
    }
}