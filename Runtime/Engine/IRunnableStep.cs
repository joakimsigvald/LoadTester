using Applique.LoadTester.Domain.Design;
using System;
using System.Threading.Tasks;

namespace Applique.LoadTester.Runtime.Engine
{
    public interface IRunnableStep
    {
        Step Blueprint { get; }
        Task<TimeSpan> Run();
    }
}