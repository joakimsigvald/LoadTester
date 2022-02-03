using Applique.LoadTester.Domain;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Logic.Runtime.Test.Engine.RestStep;
using Applique.LoadTester.Runtime.External;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using static Applique.LoadTester.Domain.Service.ConstantExpressions;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Runtime.Test.Engine.RestStep
{
    public abstract class WhenRun : RestStepTestBase<TimeSpan>
    {
        protected Exception Exception;

        private async Task<TimeSpan> ActAsync()
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

        protected override void Act()
        {
            ReturnValue = ActAsync().Result;
        }

        public class GivenRestGetReturnOk : WhenRun
        {
            [Fact]
            public void ThenReturnElapsedTime()
            {
                ArrangeAndAct();
                Assert.True(ReturnValue > TimeSpan.Zero);
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
                RestCallerMock.Verify(rc => rc.Call(It.Is<Request>(req => req.Content == $"{{\"MyProperty\":\"{AnotherString}\"}}")));
            }
        }
    }
}
