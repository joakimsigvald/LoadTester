using Newtonsoft.Json;

namespace LoadTester
{
    public class TestSuite
    {
        public string Name { get; set; }
        public Service[] Services { get; set; }
        public Scenario[] Scenarios { get; set; }
    }
}