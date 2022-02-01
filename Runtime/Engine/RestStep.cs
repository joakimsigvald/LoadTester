using System;
using System.Net.Http;
using System.Threading.Tasks;
using Applique.LoadTester.Domain;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;

namespace Applique.LoadTester.Runtime.Engine
{
    public class RestStep : RunnableStep<HttpResponseMessage>
    {
        private readonly RestStepExecutor _executor;
        private readonly IStepVerifier _stepVerifier;

        internal RestStep(
            Step step,
            IBindings bindings,
            IBindings overloads,
            RestStepExecutor executor,
            IStepVerifier stepVerifier)
            : base(step, bindings, overloads)
        {
            _executor = executor;
            _stepVerifier = stepVerifier;
        }

        protected override async Task HandleResponse(HttpResponseMessage response)
        {
            var body = response.Content is null ? null : await response.Content.ReadAsStringAsync();
            if (!_stepVerifier.IsResponseStatusValid(response.StatusCode))
                throw new RunFailed($"Expected {string.Join(", ", Blueprint.ExpectedStatusCodes)} but got {response.StatusCode}: {body}");
            if (Blueprint.Response is null)
                return;
            if (body is null)
                throw new RunFailed("Expected Response body but got null");
            _stepVerifier.VerifyResponse(Blueprint.Response, body);
        }

        protected override async Task<HttpResponseMessage> DoRun()
        {
            HttpResponseMessage lastResponse;
            var serviceHeaders = _executor.CreateServiceHeaders();
            int attempt = 0;
            do lastResponse = await Execute(++attempt, serviceHeaders);
            while (await ShouldContinue(lastResponse, attempt));
            return lastResponse;
        }

        private async Task<bool> ShouldContinue(HttpResponseMessage response, int attempt)
            => attempt < Blueprint.Times 
            && await IsSuccessful(response) 
            ? !Blueprint.BreakOnSuccess 
            : Blueprint.RetryOnFail;

        private async Task<bool> IsSuccessful(HttpResponseMessage response)
            => response != null && await _stepVerifier.IsSuccessful(response);

        private async Task<HttpResponseMessage> Execute(int attempt, Header[] serviceHeaders)
        {
            await Task.Delay(Delay);
            Console.WriteLine($"Calling {Blueprint.Endpoint}, attempt {attempt}");
            return await _executor.Execute(serviceHeaders); 
        }
    }
}