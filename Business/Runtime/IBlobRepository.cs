using System.Threading.Tasks;

namespace Applique.LoadTester.Business.Runtime
{
    public interface IBlobRepository
    {
        Task Upload(string name, string text);
    }
}