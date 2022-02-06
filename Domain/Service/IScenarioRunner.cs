using Applique.LoadTester.Core.Result;
using Applique.LoadTester.Domain.Assembly;
using System.Threading.Tasks;

namespace Applique.LoadTester.Domain.Service
{
    public interface IScenarioRunner
    {
        Task<IScenarioResult> Run(ITestSuiteScenario instantiator);
    }
}