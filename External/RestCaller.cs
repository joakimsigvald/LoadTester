using Applique.LoadTester.Domain.Service;
using Applique.LoadTester.Logic.Runtime.External;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Applique.LoadTester.External
{
    internal class RestCaller : IRestCaller
    {
        private readonly HttpClient _client;

        public RestCaller(string baseUrl) => _client = CreateClient(baseUrl);

        public async Task<RestCallResponse> Call(Request request)
        {
            var res = await _client.SendAsync(MapToHttpRequestMessage(request));
            var body = res.Content is null ? null : await res.Content.ReadAsStringAsync();
            return new RestCallResponse
            {
                StatusCode = res.StatusCode,
                Body = body
            };
        }

        private static HttpRequestMessage MapToHttpRequestMessage(Request request)
        {
            var requestMessage = new HttpRequestMessage(new(request.Method), request.Url)
            {
                Content = request.Content == null
                ? null
                : new StringContent(request.Content, Encoding.UTF8, "application/json")
            };
            foreach (var header in request.Headers)
                requestMessage.Headers.Add(header.Name, header.Value);
            return requestMessage;
        }

        private static HttpClient CreateClient(string baseUrl) => new() { BaseAddress = new(baseUrl) };
    }
}