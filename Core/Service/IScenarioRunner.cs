using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Result;
using System.Threading.Tasks;

namespace Applique.LoadTester.Core.Service
{
    public interface IScenarioRunner
    {
        Task<IScenarioResult> Run(IScenario scenario);
    }
}