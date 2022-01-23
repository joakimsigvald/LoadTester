using System.Threading.Tasks;

namespace Applique.LoadTester.Runtime.Environment
{
    public interface IBlobRepository
    {
        Task Upload(string name, string text);
    }
}