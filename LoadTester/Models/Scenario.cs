namespace LoadTester
{
    public class Scenario
    {
        public string Name { get; set; }
        public int Instances { get; set; }
        public Step[] Steps { get; set; }
        public Assert[] Asserts { get; set; }

        public RunnableScenario CreateInstance(TestSuite suite, int instanceId)
            => new RunnableScenario(suite, this, instanceId);
    }
}