using System.Threading.Tasks;

namespace Applique.LoadTester.Logic.Runtime.External
{
    public interface IBlobRepository
    {
        Task Upload(string name, string text);
    }
}