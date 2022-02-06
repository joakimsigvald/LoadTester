using Applique.LoadTester.Domain.Design;
using System;
using System.Threading.Tasks;

namespace Applique.LoadTester.Logic.Runtime.Engine
{
    public interface IRunnableStep
    {
        IStep Blueprint { get; }
        Task<TimeSpan> Run();
    }
}