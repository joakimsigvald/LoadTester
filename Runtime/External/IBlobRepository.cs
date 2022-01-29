using System.Threading.Tasks;

namespace Applique.LoadTester.Runtime.External
{
    public interface IBlobRepository
    {
        Task Upload(string name, string text);
    }
}