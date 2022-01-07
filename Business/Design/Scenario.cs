using Applique.LoadTester.Business.Runtime;
using System;

namespace Applique.LoadTester.Business.Design
{
    public class Scenario
    {
        public string Name { get; set; }
        public bool Disabled { get; set; }
        public int Instances { get; set; } = 1;
        public Step[] Steps { get; set; }
        public Assert[] Asserts { get; set; } = Array.Empty<Assert>();

        public RunnableScenario CreateInstance(TestSuite suite, int instanceId)
            => new(suite, this, instanceId);
    }
}