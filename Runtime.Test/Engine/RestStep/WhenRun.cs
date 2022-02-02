using Applique.LoadTester.Runtime.External;
using Moq;
using Xunit;
using static Applique.LoadTester.Environment.ConstantExpressions;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Runtime.Test.Engine.RestStep
{
    public abstract class WhenRun : RestStepTestBase
    {
        protected override void Act()
        {
            SUT.Run();
        }

        public class GivenTemplateAndOverloads : WhenRun
        {
            [Fact]
            public void ThenApplyOverloadsToBody()
            {
                Variables[SomeConstant] = SomeString;
                OverloadVariables[SomeConstant] = AnotherString;
                Body = new { MyProperty = Embrace(SomeConstant) };
                ArrangeAndAct();
                RestCallerMock.Verify(rc => rc.Call(It.Is<Request>(req => req.Content == $"{{\"MyProperty\":\"{AnotherString}\"}}")));
            }
        }
    }
}
