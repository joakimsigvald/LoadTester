using Applique.LoadTester.Business.External;
using Applique.LoadTester.Business.Runtime;
using System;

namespace Applique.LoadTester.Business.Design
{
    public class Scenario
    {
        public string Name { get; set; }
        public bool Disabled { get; set; }
        public int Instances { get; set; } = 1;
        public string[] Load { get; set; } = Array.Empty<string>();
        public string[] Persist { get; set; } = Array.Empty<string>();
        public Constant[] Constants { get; set; } = Array.Empty<Constant>();
        public Step[] Steps { get; set; }
        public Assert[] Asserts { get; set; } = Array.Empty<Assert>();

        public RunnableScenario CreateInstance(IFileSystem fileSystem, TestSuite suite, int instanceId)
            => new(fileSystem, suite, this, instanceId);
    }
}