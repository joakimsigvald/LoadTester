using System.Threading.Tasks;

namespace Applique.LoadTester.Domain.Environment
{
    public interface IBlobRepository
    {
        Task Upload(string name, string text);
    }
}