using System;
using System.Threading.Tasks;
using Applique.LoadTester.Domain;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;
using Applique.LoadTester.Domain.Assembly;

namespace Applique.LoadTester.Logic.Runtime.Engine
{
    public class RestStep : RunnableStep<RestCallResponse>
    {
        private readonly RestStepExecutor _executor;
        private readonly IStepVerifier _stepVerifier;

        public RestStep(
            IStep step,
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
            while (ShouldContinue(lastResponse, attempt));
            return lastResponse;
        }

        private bool ShouldContinue(RestCallResponse response, int attempt)
            => attempt < Blueprint.Times && !ShouldAbort(response);

        private bool ShouldAbort(RestCallResponse response)
            => _stepVerifier.IsSuccessful(response) ? Blueprint.BreakOnSuccess : !Blueprint.RetryOnFail;

        private async Task<RestCallResponse> Execute(int attempt, Header[] serviceHeaders)
        {
            await Task.Delay(Delay);
            Console.WriteLine($"Calling {Blueprint.Endpoint}, attempt {attempt}");
            return await _executor.Execute(serviceHeaders);
        }
    }
}