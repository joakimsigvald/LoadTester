using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain;
using Applique.LoadTester.Domain.Assembly;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;
using Applique.LoadTester.Logic.Assembly;
using Applique.LoadTester.Logic.Environment;
using Applique.LoadTester.Logic.Runtime.Engine;
using Applique.LoadTester.Logic.Runtime.External;
using Applique.WhenGivenThen.Core;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using static Applique.LoadTester.Domain.Design.ConstantExpressions;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Runtime.Test.Engine.RestStep
{
    public abstract class WhenRun : TestSubjectAsync<Logic.Runtime.Engine.RestStep, TimeSpan>
    {
        protected Exception Exception;
        protected dynamic RequestBody;
        protected JToken TemplateBody;
        protected string ResponseBody;
        protected string Template;
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

        protected override Logic.Runtime.Engine.RestStep CreateSUT()
        {
            var bindings = CreateBindings(Variables);
            var overloads = CreateBindings(OverloadVariables);
            var testSuite = MockTestSuite();
            SetupRestCallerFactory();
            var step = CreateStep();
            var executor = RestStepExecutor.Create(MockOf<IRestCallerFactory>(), testSuite, step, bindings);
            return new Logic.Runtime.Engine.RestStep(step, bindings, overloads, executor, new StepVerifier(step, bindings));
        }

        private void SetupRestCallerFactory()
        {
            Mocked<IRestCaller>().SetReturnsDefault(Task.FromResult(new RestCallResponse
            {
                StatusCode = RestStatusCode,
                Body = ResponseBody
            }));
            Mocked<IRestCallerFactory>().Setup(rcf => rcf.Create(It.IsAny<string>())).Returns(MockOf<IRestCaller>());
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

        protected async override Task<TimeSpan> ActAsync()
        {
            try
            {
                return await SUT.Run();
            }
            catch (Exception ex)
            {
                Exception = ex;
                return default;
            }
        }

        public class GivenRestGetReturnOk : WhenRun
        {
            [Fact]
            public void ThenReturnElapsedTime()
            {
                ArrangeAndAct();
                //TODO: remove dependency on timer
                //Assert.True(Result > TimeSpan.Zero);
            }
        }

        public class GivenRestGetReturnUnexpectedStatusCodeError : WhenRun
        {
            [Theory]
            [InlineData(HttpStatusCode.OK, HttpStatusCode.NotFound)]
            [InlineData(HttpStatusCode.OK, HttpStatusCode.Accepted)]
            [InlineData(HttpStatusCode.NotFound, HttpStatusCode.OK)]
            [InlineData(HttpStatusCode.InternalServerError, HttpStatusCode.BadGateway)]
            public void ThenThrowRunFailed(HttpStatusCode expected, HttpStatusCode returned)
            {
                ExpectedStatusCode = expected;
                RestStatusCode = returned;
                ArrangeAndAct();
                Assert.True(Exception is RunFailed, $"Exception was {Exception?.GetType()}");
            }
        }

        public class GivenRestGetReturnUnexpectedValue : WhenRun
        {
            [Fact]
            public void ThenThrowRunFailed()
            {
                TemplateBody = new JObject();
                var dtb = (dynamic)TemplateBody;
                dtb.MyProperty = SomeString;
                ResponseBody = $"{{\"MyProperty\":\"{AnotherString}\"}}";
                ArrangeAndAct();
                Assert.True(Exception is RunFailed, $"Exception was {Exception?.GetType()}");
            }
        }

        public class GivenOverloads : WhenRun
        {
            [Fact]
            public void ThenApplyOverloadsToBody()
            {
                Variables[SomeConstant] = SomeString;
                OverloadVariables[SomeConstant] = AnotherString;
                RequestBody = new { MyProperty = Embrace(SomeConstant) };
                ArrangeAndAct();
                Mocked<IRestCaller>().Verify(rc => rc.Call(It.Is<Request>(req => req.Content == $"{{\"MyProperty\":\"{AnotherString}\"}}")));
            }
        }
    }
}
