using System.Net.Http;
using System.Threading.Tasks;

namespace Applique.LoadTester.Domain.Environment
{
    public interface IRestCaller
    {
        Task<HttpResponseMessage> Call(object body, string args);
    }
}