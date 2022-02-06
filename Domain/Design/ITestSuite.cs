using Applique.LoadTester.Core.Design;
using System.Collections.Generic;

namespace Applique.LoadTester.Domain.Design
{
    public interface ITestSuite
    {
        string Name { get; }
        Service[] Services { get; }
        Constant[] Constants { get; }
        IEnumerable<IScenario> ScenariosToRun { get; }
        IStep GetStepTemplate(string name);
        Blob GetBlob(string name);
    }
}