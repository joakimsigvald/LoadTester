using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Applique.LoadTester.Domain.Service
{
    public interface IStepVerifier
    {
        void VerifyResponse(JToken pattern, string source, string prefix = "");

        Task<bool> IsSuccessful(RestCallResponse response);

        bool IsResponseStatusValid(HttpStatusCode actualStatus);
    }
}