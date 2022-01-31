using Newtonsoft.Json.Linq;

namespace Applique.LoadTester.Domain.Service
{
    public interface IValueVerifier
    {
        void VerifyValue(string prefix, JProperty expected, string actualValue);
    }
}