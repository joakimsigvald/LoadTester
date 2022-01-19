using System.Net.Http;
using System.Threading.Tasks;

namespace Applique.LoadTester.Business.Runtime
{
    public interface IRestCaller
    {
        Task<HttpResponseMessage> Call(object body, string args);
    }
}