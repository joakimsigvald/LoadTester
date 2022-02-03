using Applique.LoadTester.Domain.Service;
using System.Threading.Tasks;

namespace Applique.LoadTester.Runtime.External
{

    public interface IRestCaller
    {
        Task<RestCallResponse> Call(Request request);
    }
}