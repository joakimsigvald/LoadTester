using Applique.LoadTester.Domain.Assembly;
using Applique.WhenGivenThen.Core;
using Applique.LoadTester.Core.Design;
using System.Linq;
using Xunit;
using System.Net;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Logic.Assembly.Test.Step;

public abstract class WhenMerge : TestSubject<Assembly.Step, IStep>
{
    protected Assembly.Step Step = new();
    protected Assembly.Step Other = new();

    protected override Assembly.Step CreateSUT() => Step;

    protected override void Act() => CollectResult(() => SUT.MergeWith(Other));

    public class WhenMerged : WhenMerge { public WhenMerged() => ArrangeAndAct(); }

    public class GivenBothHasDifferentArgs : WhenMerged
    {
        protected override void Given() => (Step.Args, Other.Args) = ("a=1", "b=2");
        [Fact] public void ThenAppendGetArgsFromOther() => Assert.Equal("a=1&b=2", Result.Args);
    }

    public class GivenBothHasSameArg : WhenMerged
    {
        protected override void Given() => (Step.Args, Other.Args) = ("a=1", "a=2");
        [Fact] public void ThenGetArgValueFromOther() => Assert.Equal("a=2", Result.Args);
    }

    public class GivenOnlyStepHasArgs : WhenMerged
    {
        protected override void Given() => Step.Args = "a=1";
        [Fact] public void ThenGetArgsFromStep() => Assert.Equal(Step.Args, Result.Args);
    }

    public class GivenOnlyOtherHasArgs : WhenMerged
    {
        protected override void Given() => Other.Args = "a=1";
        [Fact] public void ThenGetArgsFromOther() => Assert.Equal(Other.Args, Result.Args);
    }

    public class GivenStepHasDuplicatedArgKeys : WhenMerged
    {
        protected override void Given() => Step.Args = "a=1&a=2";
        [Fact] public void ThenThenConvertToCommaSeparatedValues() => Assert.Equal("a=1,2", Result.Args);
    }

    public class GivenOnlyOtherHasBody : WhenMerged
    {
        protected override void Given() => Other.Body = new { A = "B" };
        [Fact] public void ThenGetBodyFromOther() => Assert.Same(Other.Body, Result.Body);
    }

    public class GivenBothHasBody : WhenMerged
    {
        protected override void Given() => (Step.Body, Other.Body) = (new { B = "C" }, new { A = "B" });
        [Fact] public void ThenGetBodyFromOther() => Assert.Same(Other.Body, Result.Body);
    }

    public class GivenBothHasSameConstant : WhenMerged
    {
        protected override void Given()
            => (Step.Constants, Other.Constants)
            = (CreateConstants((SomeConstant, SomeString)), CreateConstants((SomeConstant, AnotherString)));
        [Fact] public void ThenHasTheConstant() => Assert.Equal(SomeConstant, Result.Constants.Single().Name);
        [Fact] public void ThenGetValueFromOther() => Assert.Equal(AnotherString, Result.Constants[0].Value);
    }

    public class GivenBothHasEndpoint : WhenMerged
    {
        protected override void Given() => (Step.Endpoint, Other.Endpoint) = ("somewhere", "elsewhere");
        [Fact] public void ThenGetEndpointFromOther() => Assert.Equal(Other.Endpoint, Result.Endpoint);
    }

    public class GivenStepEndpointIsNull : WhenMerged
    {
        protected override void Given() => Other.Endpoint = "elsewhere";
        [Fact] public void ThenUseEndpointFromOther() => Assert.Equal(Other.Endpoint, Result.Endpoint);
    }

    public class GivenOtherEndpointIsNull : WhenMerged
    {
        protected override void Given() => Step.Endpoint = "somewhere";
        [Fact] public void ThenUseEndpointFromStep() => Assert.Equal(Step.Endpoint, Result.Endpoint);
    }

    public class GivenBothHasStatusCodes : WhenMerged
    {
        protected override void Given() 
            => (Step.ExpectedStatusCodes, Other.ExpectedStatusCodes) 
            = (new[] { HttpStatusCode.Accepted }, new[] { HttpStatusCode.NotFound });
        [Fact] public void ThenGetStatusCodesFromStep() => Assert.Equal(Step.ExpectedStatusCodes, Result.ExpectedStatusCodes);
    }

    public class GivenOnlyOtherHasResponse : WhenMerged
    {
        protected override void Given() => Other.Response = new { A = "B" };
        [Fact] public void ThenUseEndpointFromOther() => Assert.Same(Other.Response, Result.Response);
    }

    public class GivenBothHasResponse : WhenMerged
    {
        protected override void Given() => (Step.Response, Other.Response) = (new { B = "C" }, new { B = "C" });
        [Fact] public void ThenGetResponseFromOther() => Assert.Same(Other.Response, Result.Response);
    }

    public class GivenBothHasBreakOnSuccess : WhenMerge
    {
        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void ThenGetBreakOnSuccessFromStep(bool inStep, bool inOther)
        {
            Step.BreakOnSuccess = inStep;
            Other.BreakOnSuccess = inOther;
            ArrangeAndAct();
            Assert.Equal(inStep, Result.BreakOnSuccess);
        }
    }

    public class GivenBothHasRetryOnFail : WhenMerge
    {
        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void ThenGetRetryOnFailFromStep(bool inStep, bool inOther)
        {
            Step.RetryOnFail = inStep;
            Other.RetryOnFail = inOther;
            ArrangeAndAct();
            Assert.Equal(inStep, Result.RetryOnFail);
        }
    }

    public class GivenBothHasDelayMs : WhenMerge
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        [InlineData(2, 2)]
        public void ThenGetDelayMsFromStep(int inStep, int inOther)
        {
            Step.DelayMs = inStep;
            Other.DelayMs = inOther;
            ArrangeAndAct();
            Assert.Equal(inStep, Result.DelayMs);
        }
    }

    public class GivenBothHasTimes : WhenMerge
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        [InlineData(2, 2)]
        public void ThenGetTimesFromStep(int inStep, int inOther)
        {
            Step.Times = inStep;
            Other.Times = inOther;
            ArrangeAndAct();
            Assert.Equal(inStep, Result.Times);
        }
    }

    public class GivenBothHaveType : WhenMerge
    {
        [Theory]
        [InlineData(StepType.Blob, StepType.Blob)]
        [InlineData(StepType.Blob, StepType.Rest)]
        [InlineData(StepType.Rest, StepType.Blob)]
        [InlineData(StepType.Rest, StepType.Rest)]
        public void ThenGetTypeFromStep(StepType inStep, StepType inOther)
        {
            Step.Type = inStep;
            Other.Type = inOther;
            ArrangeAndAct();
            Assert.Equal(inStep, Result.Type);
        }
    }

    protected static Constant[] CreateConstants(params (string name, string value)[] namedValues)
        => namedValues.Select(nv => new Constant { Name = nv.name, Value = nv.value }).ToArray();
}