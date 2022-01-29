using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Applique.LoadTester.Domain
{
    public interface IStepVerifier
    {
        void VerifyResponse(JToken pattern, string source, string prefix = "");

        Task<bool> IsSuccessful(HttpResponseMessage response);

        bool IsResponseStatusValid(HttpStatusCode actualStatus);
    }
}