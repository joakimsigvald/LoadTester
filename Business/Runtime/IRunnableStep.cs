using Applique.LoadTester.Business.Design;
using System;
using System.Threading.Tasks;

namespace Applique.LoadTester.Business.Runtime
{
    public interface IRunnableStep
    {
        Step Blueprint { get; }
        Task<TimeSpan> Run();
    }
}