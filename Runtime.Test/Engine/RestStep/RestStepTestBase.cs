using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;
using Applique.LoadTester.Logic.Assembly;
using Applique.LoadTester.Logic.Environment;
using Applique.LoadTester.Logic.Runtime.Engine;
using Applique.LoadTester.Logic.Runtime.External;
using Applique.LoadTester.Test;
using Moq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Applique.LoadTester.Logic.Runtime.Test.Engine.RestStep
{
    public abstract class RestStepTestBase : TestBase<Runtime.Engine.RestStep>
    {
        protected dynamic RequestBody;
        protected JToken TemplateBody;
        protected string ResponseBody;
        protected string Template;
        protected Mock<IRestCaller> RestCallerMock = new();
        protected IDictionary<string, object> Variables = new Dictionary<string, object>();
        protected IDictionary<string, object> OverloadVariables = new Dictionary<string, object>();
        protected Service[] Services = new[]
            {
                new Service
                {
                    Name = "AService",
                    Endpoints = new[] { new Endpoint { Name = "AMethod"} }
                }
            };
        protected string StepEndpoint = "AService.AMethod";
        protected HttpStatusCode ExpectedStatusCode = HttpStatusCode.OK;
        protected HttpStatusCode RestStatusCode = HttpStatusCode.OK;

        protected override Runtime.Engine.RestStep CreateSUT()
        {
            var bindings = CreateBindings(Variables);
            var overloads = CreateBindings(OverloadVariables);
            var restCallerFactory = MockRestCallerFactory();
            var testSuite = MockTestSuite();
            var step = CreateStep();
            var executor = RestStepExecutor.Create(restCallerFactory, testSuite, step, bindings);
            var stepVerifierMock = new Mock<IStepVerifier>();
            stepVerifierMock.SetReturnsDefault(true);
            return new Runtime.Engine.RestStep(step, bindings, overloads, executor, new StepVerifier(step, bindings));
        }

        private IRestCallerFactory MockRestCallerFactory()
        {
            RestCallerMock.SetReturnsDefault(Task.FromResult(new RestCallResponse
            {
                StatusCode = RestStatusCode,
                Body = ResponseBody
            }));
            var mock = new Mock<IRestCallerFactory>();
            mock.Setup(rcf => rcf.Create(It.IsAny<string>()))
                .Returns(RestCallerMock.Object);
            return mock.Object;
        }

        private ITestSuite MockTestSuite()
        {
            var mock = new Mock<ITestSuite>();
            mock.Setup(suite => suite.Services).Returns(Services);
            return mock.Object;
        }

        private Step CreateStep()
            => new()
            {
                Endpoint = StepEndpoint,
                Body = RequestBody,
                ExpectedStatusCodes = new[] { ExpectedStatusCode },
                Response = TemplateBody
            };

        protected static IBindings CreateBindings(IDictionary<string, object> variables)
            => new Bindings(new BindingVariables(variables));
    }

    public abstract class RestStepTestBase<TReturn> : RestStepTestBase
    {
        protected TReturn ReturnValue;
    }
}