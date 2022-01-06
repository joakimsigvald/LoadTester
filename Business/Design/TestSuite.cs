namespace Applique.LoadTester.Business.Design
{
    public class TestSuite
    {
        public string Name { get; set; }
        public Constant[] Constants { get; set; }
        public Service[] Services { get; set; }
        public Scenario[] Scenarios { get; set; }
    }
}