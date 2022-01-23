using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Result;
using System.Threading.Tasks;

namespace Applique.LoadTester.Domain.Engine
{
    public interface IScenarioRunner
    {
        Task<IScenarioResult> Run(IScenario scenario);
    }
}