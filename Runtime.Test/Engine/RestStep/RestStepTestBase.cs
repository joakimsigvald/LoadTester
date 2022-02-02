using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;
using Applique.LoadTester.Environment;
using Applique.LoadTester.Runtime.Engine;
using Applique.LoadTester.Runtime.External;
using Applique.LoadTester.Test;
using Moq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Applique.LoadTester.Runtime.Test.Engine.RestStep
{
    public abstract class RestStepTestBase : TestBase<Runtime.Engine.RestStep>
    {
        protected dynamic Body;
        protected string Template;
        protected Mock<IRestCaller> RestCallerMock = new();
        protected IDictionary<string, object> Variables = new Dictionary<string, object>();
        protected IDictionary<string, object> OverloadVariables = new Dictionary<string, object>();

        protected override Runtime.Engine.RestStep CreateSUT()
        {
            var bindings = CreateBindings(Variables);
            var overloads = CreateBindings(OverloadVariables);
            RestCallerMock.SetReturnsDefault(Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)));
            var restCallerFactoryMock = new Mock<IRestCallerFactory>();
            restCallerFactoryMock.Setup(rcf => rcf.Create(It.IsAny<string>()))
                .Returns(RestCallerMock.Object);
            var testSuiteMock = new Mock<ITestSuite>();
            testSuiteMock.Setup(suite => suite.Services).Returns(new[]
            {
                new Service
                {
                    Name = "AService",
                    Endpoints = new[] { new Endpoint { Name = "AMethod"} }
                }
            });
            var stepTemplate = new Step
            {
                Endpoint = "AService.AMethod",
                Body = Body
            };
            var step = new Step
            {
                Endpoint = "AService.AMethod",
                Body = Body
            };
            var executor = RestStepExecutor.Create(restCallerFactoryMock.Object, testSuiteMock.Object, step, bindings);
            var stepVerifierMock = new Mock<IStepVerifier>();
            stepVerifierMock.SetReturnsDefault(true);
            return new Runtime.Engine.RestStep(step, bindings, overloads, executor, stepVerifierMock.Object);
        }

        protected static IBindings CreateBindings(IDictionary<string, object> variables)
            => new Bindings(new BindingVariables(variables));
    }

    public abstract class BindingsTestBase<TReturn> : RestStepTestBase
    {
        protected TReturn ReturnValue;
    }
}