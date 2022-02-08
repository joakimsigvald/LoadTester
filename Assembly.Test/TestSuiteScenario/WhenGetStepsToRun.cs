﻿using Applique.LoadTester.Domain.Assembly;
using System.Linq;
using System.Net;
using Xunit;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Logic.Assembly.Test.TestSuiteScenario
{
    public abstract class WhenGetStepsToRun : TestSuiteScenarioTestBase<IStep>
    {
        protected override void Act() => ReturnValue = SUT.GetStepsToRun().Single();

        public class GivenNoTemplate : WhenGetStepsToRun
        {
            [Fact]
            public void ThenReturnStepFromScript()
            {
                ArrangeAndAct();
                Assert.Same(Step, ReturnValue);
            }
        }

        public abstract class GivenTemplate : WhenGetStepsToRun
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
                base.Given();
            }
        }

        public class GivenArgs : GivenTemplate
        {
            [Theory]
            [InlineData(null, null)]
            [InlineData(null, "?b=2")]
            [InlineData("?a=1", null)]
            [InlineData("?a=1", "?b=2")]
            public void ThenGetArgsFromTemplate(string fromTemplate, string fromStep)
            {
                TemplateStep.Args = fromTemplate;
                Step.Args = fromStep;
                ArrangeAndAct();
                Assert.Equal(fromTemplate, ReturnValue.Args);
            }
        }

        public class GivenOnlyTemplateHasBody : GivenTemplate
        {
            [Fact]
            public void ThenGetBodyFromTemplate()
            {
                TemplateStep.Body = new { A = "B" };
                ArrangeAndAct();
                Assert.Same(TemplateStep.Body, ReturnValue.Body);
            }
        }

        public class GivenStepAndTemplateHasBody : GivenTemplate
        {
            [Fact]
            public void ThenGetBodyFromStep()
            {
                TemplateStep.Body = new { A = "B" };
                Step.Body = new { B = "C" };
                ArrangeAndAct();
                Assert.Same(Step.Body, ReturnValue.Body);
            }
        }

        public class GivenStepAndTemplateHasConstants : GivenTemplate
        {
            [Fact]
            public void ThenMergeConstants()
            {
                TemplateStep.Constants = CreateConstants((SomeConstant, AnotherString));
                Step.Constants = CreateConstants((SomeConstant, SomeString));
                ArrangeAndAct();
                var actual = ReturnValue.Constants.Single(c => c.Name == SomeConstant).Value;
                Assert.Equal(SomeString, actual);
            }
        }

        public class GivenEndpoint : GivenTemplate
        {
            [Theory]
            [InlineData(null, null)]
            [InlineData(null, "elsewhere")]
            [InlineData("somewhere", null)]
            [InlineData("somewhere", "elsewhere")]
            public void ThenUseTemplateAsFallback(string fromTemplate, string fromStep)
            {
                TemplateStep.Endpoint = fromTemplate;
                Step.Endpoint = fromStep;
                ArrangeAndAct();
                Assert.Equal(fromStep ?? fromTemplate, ReturnValue.Endpoint);
            }
        }

        public class GivenStatusCodes : GivenTemplate
        {
            [Fact]
            public void ThenGetStatusCodesFromTemplate()
            {
                TemplateStep.ExpectedStatusCodes = new[] { HttpStatusCode.NotFound };
                Step.ExpectedStatusCodes = new[] { HttpStatusCode.Accepted };
                ArrangeAndAct();
                Assert.Equal(TemplateStep.ExpectedStatusCodes, ReturnValue.ExpectedStatusCodes);
            }
        }

        public class GivenOnlyTemplateHasResponse : GivenTemplate
        {
            [Fact]
            public void ThenGetResponseFromTemplate()
            {
                TemplateStep.Response = new { A = "B" };
                ArrangeAndAct();
                Assert.Same(TemplateStep.Response, ReturnValue.Response);
            }
        }

        public class GivenStepAndTemplateHasResponse : GivenTemplate
        {
            [Fact]
            public void ThenGetResponseFromStep()
            {
                TemplateStep.Response = new { A = "B" };
                Step.Response = new { B = "C" };
                ArrangeAndAct();
                Assert.Same(Step.Response, ReturnValue.Response);
            }
        }

        public class GivenBreakOnSuccess : GivenTemplate
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
                Assert.Equal(fromTemplate, ReturnValue.BreakOnSuccess);
            }
        }

        public class GivenRetryOnFail : GivenTemplate
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
                Assert.Equal(fromTemplate, ReturnValue.RetryOnFail);
            }
        }

        public class GivenDelayMs : GivenTemplate
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
                Assert.Equal(fromTemplate, ReturnValue.DelayMs);
            }
        }

        public class GivenTimes : GivenTemplate
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
                Assert.Equal(fromTemplate, ReturnValue.Times);
            }
        }

        public class GivenType : GivenTemplate
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
                Assert.Equal(fromTemplate, ReturnValue.Type);
            }
        }
    }
}