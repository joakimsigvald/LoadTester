using System.Net.Http;
using System.Threading.Tasks;

namespace Applique.LoadTester.Runtime.External
{
    public interface IRestCaller
    {
        Task<HttpResponseMessage> Call(Request request);
    }
}