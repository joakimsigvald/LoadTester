using System;
using System.Net.Http;
using System.Threading.Tasks;
using Applique.LoadTester.Domain;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;
using Applique.LoadTester.Runtime.Engine;

namespace Applique.LoadTester.Logic.Runtime.Engine
{
    public class RestStep : RunnableStep<RestCallResponse>
    {
        private readonly RestStepExecutor _executor;
        private readonly IStepVerifier _stepVerifier;

        public RestStep(
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

        protected override void HandleResponse(RestCallResponse response)
        {
            if (!_stepVerifier.IsResponseStatusValid(response.StatusCode))
                throw new RunFailed($"Expected {string.Join(", ", Blueprint.ExpectedStatusCodes)} but got {response.StatusCode}: {response.Body}");
            if (Blueprint.Response is null)
                return;
            if (response.Body is null)
                throw new RunFailed("Expected Response body but got null");
            _stepVerifier.VerifyResponse(Blueprint.Response, response.Body);
        }

        protected override async Task<RestCallResponse> DoRun()
        {
            RestCallResponse lastResponse;
            var serviceHeaders = _executor.CreateServiceHeaders();
            int attempt = 0;
            do lastResponse = await Execute(++attempt, serviceHeaders);
            while (attempt < Blueprint.Times && await ShouldContinue(lastResponse));
            return lastResponse;
        }

        private async Task<bool> ShouldContinue(RestCallResponse response)
            => await IsSuccessful(response) ? !Blueprint.BreakOnSuccess : Blueprint.RetryOnFail;

        private async Task<bool> IsSuccessful(RestCallResponse response)
            => response != null && await _stepVerifier.IsSuccessful(response);

        private async Task<RestCallResponse> Execute(int attempt, Header[] serviceHeaders)
        {
            await Task.Delay(Delay);
            Console.WriteLine($"Calling {Blueprint.Endpoint}, attempt {attempt}");
            return await _executor.Execute(serviceHeaders);
        }
    }
}