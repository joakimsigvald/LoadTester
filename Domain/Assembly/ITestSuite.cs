using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Domain.Design;
using System.Collections.Generic;

namespace Applique.LoadTester.Domain.Assembly
{
    public interface ITestSuite
    {
        string Name { get; }
        Design.Service[] Services { get; }
        Constant[] Constants { get; }
        IEnumerable<ITestSuiteScenario> ScenarioWrappers { get; }
        Blob GetBlob(string name);
    }
}