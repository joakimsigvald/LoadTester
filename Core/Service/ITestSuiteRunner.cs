using System.Threading.Tasks;
using Applique.LoadTester.Core.Result;

namespace Applique.LoadTester.Core.Service
{
    public interface ITestSuiteRunner
    {
        public string TestSuiteName { get; }
        Task<TestSuiteResult> Run();
    }
}