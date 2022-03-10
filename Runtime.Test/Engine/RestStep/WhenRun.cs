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

        protected override void Setup()
        {
            Mocked<ITestSuite>().Setup(suite => suite.Services).Returns(Services);
            SetupRestCallerFactory();
        }

        protected override Logic.Runtime.Engine.RestStep CreateSUT()
        {
            var bindings = CreateBindings(Variables);
            var overloads = CreateBindings(OverloadVariables);
            var step = CreateStep();
            var executor = RestStepExecutor.Create(MockOf<IRestCallerFactory>(), MockOf<ITestSuite>(), step, bindings);
            return new Logic.Runtime.Engine.RestStep(step, bindings, overloads, executor, new StepVerifier(step, bindings));
        }

        protected override Task<TimeSpan> ActAsync() => SUT.Run();

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

        public class GivenGetUnexpectedStatusCode : WhenRun
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
                Assert.Throws<RunFailed>(ArrangeAndAct);
            }
        }

        public class GivenRestGetReturnUnexpectedValue : WhenRun
        {
            protected override void Given()
            {
                TemplateBody = new JObject();
                var dtb = (dynamic)TemplateBody;
                dtb.MyProperty = SomeString;
                ResponseBody = $"{{\"MyProperty\":\"{AnotherString}\"}}";
            }

            [Fact] public void ThenThrowRunFailed() => Assert.Throws<VerificationFailed>(ArrangeAndAct);
        }

        public class GivenOverloads : WhenRun
        {
            protected override void Given()
            {
                Variables[SomeConstant] = SomeString;
                OverloadVariables[SomeConstant] = AnotherString;
                RequestBody = new { MyProperty = Embrace(SomeConstant) };
            }

            public GivenOverloads() => ArrangeAndAct();

            [Fact]
            public void ThenApplyOverloadsToBody()
                => Mocked<IRestCaller>()
                .Verify(rc => rc.Call(It.Is<Request>(
                    req => req.Content == $"{{\"MyProperty\":\"{AnotherString}\"}}")));
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

        private Step CreateStep()
            => new()
            {
                Endpoint = StepEndpoint,
                Body = RequestBody,
                ExpectedStatusCodes = new[] { ExpectedStatusCode },
                Response = TemplateBody
            };

        private static IBindings CreateBindings(IDictionary<string, object> variables)
            => new Bindings(new BindingVariables(variables));
    }
}