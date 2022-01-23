using Applique.LoadTester.Design;
using System.Net.Http;

namespace Applique.LoadTester.Runtime.Environment
{
    public static class ServiceExtensions
    {
        public static HttpClient CreateClient(this Service service, Bindings bindings)
        {
            var client = new HttpClient
            {
                BaseAddress = service.BaseAddress,
            };
            client.DefaultRequestHeaders.Add(service.ApiKey.Name, service.ApiKey.Value);
            foreach (var header in service.DefaultHeaders)
                client.DefaultRequestHeaders.Add(header.Name, bindings.SubstituteVariables(header.Value));
            return client;
        }
    }
}