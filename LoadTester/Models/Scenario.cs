﻿using System;

namespace LoadTester
{
    public class Scenario
    {
        public string Name { get; set; }
        public int Instances { get; set; } = 1;
        public Step[] Steps { get; set; }
        public Assert[] Asserts { get; set; } = Array.Empty<Assert>();

        public RunnableScenario CreateInstance(TestSuite suite, int instanceId)
            => new RunnableScenario(suite, this, instanceId);
    }
}