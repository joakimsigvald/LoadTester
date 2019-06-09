namespace LoadTester
{
    public class Scenario
    {
        public string Name { get; set; }
        public int Instances { get; set; }
        public Step[] Steps { get; set; }

        public RunnableScenario CreateInstance(TestSuite suite)
            => new RunnableScenario(suite, this);
    }
}