using Applique.LoadTester.Domain.Assembly;
using System.Linq;
using System.Net;
using Xunit;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Logic.Assembly.Test.TestSuiteScenario;

public abstract class GivenOneTemplate : WhenGetStepsToRun
{
    protected Step TemplateStep = new();

    protected override void Given()
    {
        Step.Template = "TheTemplate";
        StepTemplates = new[]
        {
            new StepTemplate
            {
                Name = Step.Template,
                Step = TemplateStep
            }
        };
    }

    public class GivenTemplateHasArgs : GivenOneTemplate
    {
        [Fact]
        public void ThenGetArgsFromTemplate()
        {
            TemplateStep.Args = "?a=1";
            Step.Args = "?b=2";
            ArrangeAndAct();
            Assert.Equal(TemplateStep.Args, Result.Args);
        }
    }

    public class GivenOnlyStepHasArgs : GivenOneTemplate
    {
        [Fact]
        public void ThenArgsIsEmpty()
        {
            Step.Args = "?a=1";
            ArrangeAndAct();
            Assert.Empty(Result.Args);
        }
    }

    public class GivenOnlyTemplateHasBody : GivenOneTemplate
    {
        [Fact]
        public void ThenGetBodyFromTemplate()
        {
            TemplateStep.Body = new { A = "B" };
            ArrangeAndAct();
            Assert.Same(TemplateStep.Body, Result.Body);
        }
    }

    public class GivenStepAndTemplateHasBody : GivenOneTemplate
    {
        [Fact]
        public void ThenGetBodyFromStep()
        {
            TemplateStep.Body = new { A = "B" };
            Step.Body = new { B = "C" };
            ArrangeAndAct();
            Assert.Same(Step.Body, Result.Body);
        }
    }

    public class GivenStepAndTemplateHasConstants : GivenOneTemplate
    {
        [Fact]
        public void ThenMergeConstants()
        {
            TemplateStep.Constants = CreateConstants((SomeConstant, AnotherString));
            Step.Constants = CreateConstants((SomeConstant, SomeString));
            ArrangeAndAct();
            var actual = Result.Constants.Single(c => c.Name == SomeConstant).Value;
            Assert.Equal(SomeString, actual);
        }
    }

    public class GivenStepAndTemplateHasEndpoint : GivenOneTemplate
    {
        [Fact]
        public void ThenUseEndpointFromStep()
        {
            Step.Endpoint = "somewhere";
            TemplateStep.Endpoint = "elsewhere";
            ArrangeAndAct();
            Assert.Equal(Step.Endpoint, Result.Endpoint);
        }
    }

    public class GivenStepEndpointIsNull : GivenOneTemplate
    {
        [Fact]
        public void ThenUseEndpointFromTemplate()
        {
            TemplateStep.Endpoint = "elsewhere";
            ArrangeAndAct();
            Assert.Equal(TemplateStep.Endpoint, Result.Endpoint);
        }
    }

    public class GivenStatusCodesInTemplate : GivenOneTemplate
    {
        [Fact]
        public void ThenGetStatusCodesFromTemplate()
        {
            TemplateStep.ExpectedStatusCodes = new[] { HttpStatusCode.NotFound };
            Step.ExpectedStatusCodes = new[] { HttpStatusCode.Accepted };
            ArrangeAndAct();
            Assert.Equal(TemplateStep.ExpectedStatusCodes, Result.ExpectedStatusCodes);
        }
    }

    public class GivenOnlyTemplateHasResponse : GivenOneTemplate
    {
        [Fact]
        public void ThenGetResponseFromTemplate()
        {
            TemplateStep.Response = new { A = "B" };
            ArrangeAndAct();
            Assert.Same(TemplateStep.Response, Result.Response);
        }
    }

    public class GivenStepAndTemplateHasResponse : GivenOneTemplate
    {
        [Fact]
        public void ThenGetResponseFromStep()
        {
            TemplateStep.Response = new { A = "B" };
            Step.Response = new { B = "C" };
            ArrangeAndAct();
            Assert.Same(Step.Response, Result.Response);
        }
    }

    public class GivenBreakOnSuccessInTemplate : GivenOneTemplate
    {
        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void ThenGetBreakOnSuccessFromTemplate(bool fromTemplate, bool fromStep)
        {
            TemplateStep.BreakOnSuccess = fromTemplate;
            Step.BreakOnSuccess = fromStep;
            ArrangeAndAct();
            Assert.Equal(fromTemplate, Result.BreakOnSuccess);
        }
    }

    public class GivenRetryOnFailInTemplate : GivenOneTemplate
    {
        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void ThenGetRetryOnFailFromTemplate(bool fromTemplate, bool fromStep)
        {
            TemplateStep.RetryOnFail = fromTemplate;
            Step.RetryOnFail = fromStep;
            ArrangeAndAct();
            Assert.Equal(fromTemplate, Result.RetryOnFail);
        }
    }

    public class GivenDelayMsInTemplate : GivenOneTemplate
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        [InlineData(2, 2)]
        public void ThenGetDelayMsFromTemplate(int fromTemplate, int fromStep)
        {
            TemplateStep.DelayMs = fromTemplate;
            Step.DelayMs = fromStep;
            ArrangeAndAct();
            Assert.Equal(fromTemplate, Result.DelayMs);
        }
    }

    public class GivenTimesInTemplate : GivenOneTemplate
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        [InlineData(2, 2)]
        public void ThenGetTimesFromTemplate(int fromTemplate, int fromStep)
        {
            TemplateStep.Times = fromTemplate;
            Step.Times = fromStep;
            ArrangeAndAct();
            Assert.Equal(fromTemplate, Result.Times);
        }
    }

    public class GivenTypeInTemplate : GivenOneTemplate
    {
        [Theory]
        [InlineData(StepType.Blob, StepType.Blob)]
        [InlineData(StepType.Blob, StepType.Rest)]
        [InlineData(StepType.Rest, StepType.Blob)]
        [InlineData(StepType.Rest, StepType.Rest)]
        public void ThenGetTypeFromTemplate(StepType fromTemplate, StepType fromStep)
        {
            TemplateStep.Type = fromTemplate;
            Step.Type = fromStep;
            ArrangeAndAct();
            Assert.Equal(fromTemplate, Result.Type);
        }
    }
}