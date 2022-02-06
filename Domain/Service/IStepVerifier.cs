using Newtonsoft.Json.Linq;
using System.Net;

namespace Applique.LoadTester.Domain.Service
{
    public interface IStepVerifier
    {
        void VerifyResponse(JToken pattern, string source, string prefix = "");

        bool IsSuccessful(RestCallResponse response);

        bool IsResponseStatusValid(HttpStatusCode actualStatus);
    }
}