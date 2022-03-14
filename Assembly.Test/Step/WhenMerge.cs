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

    public class GivenBothHasDifferentArgs : WhenMerge
    {
        protected override void Given() => (Step.Args, Other.Args) = ("?a=1", "?b=2");
        public GivenBothHasDifferentArgs() => ArrangeAndAct();
        [Fact] public void ThenAppendGetArgsFromOther() => Assert.Equal("?a=1&b=2", Result.Args);
    }

    public class GivenBothHasSameArg : WhenMerge
    {
        protected override void Given() => (Step.Args, Other.Args) = ("?a=1", "?a=2");
        public GivenBothHasSameArg() => ArrangeAndAct();
        [Fact] public void ThenGetArgValueFromOther() => Assert.Equal("?a=2", Result.Args);
    }

    public class GivenOnlyStepHasArgs : WhenMerge
    {
        protected override void Given() => Step.Args = "?a=1";
        public GivenOnlyStepHasArgs() => ArrangeAndAct();
        [Fact] public void ThenGetArgsFromStep() => Assert.Equal(Step.Args, Result.Args);
    }

    public class GivenOnlyOtherHasArgs : WhenMerge
    {
        protected override void Given() => Other.Args = "?a=1";
        public GivenOnlyOtherHasArgs() => ArrangeAndAct();
        [Fact] public void ThenGetArgsFromOther() => Assert.Equal(Other.Args, Result.Args);
    }

    public class GivenStepHasDuplicatedArgKeys : WhenMerge
    {
        protected override void Given() => Step.Args = "?a=1&a=2";
        public GivenStepHasDuplicatedArgKeys() => ArrangeAndAct();
        [Fact] public void ThenThenConvertToCommaSeparatedValues() => Assert.Equal("?a=1,2", Result.Args);
    }

    public class GivenOnlyOtherHasBody : WhenMerge
    {
        [Fact]
        public void ThenGetBodyFromOther()
        {
            Other.Body = new { A = "B" };
            ArrangeAndAct();
            Assert.Same(Other.Body, Result.Body);
        }
    }

    public class GivenBothHasBody : WhenMerge
    {
        [Fact]
        public void ThenGetBodyFromOther()
        {
            Step.Body = new { B = "C" };
            Other.Body = new { A = "B" };
            ArrangeAndAct();
            Assert.Same(Other.Body, Result.Body);
        }
    }

    public class GivenBothHasSameConstant : WhenMerge
    {
        [Fact]
        public void ThenGetConstantValueFromOther()
        {
            Step.Constants = CreateConstants((SomeConstant, SomeString));
            Other.Constants = CreateConstants((SomeConstant, AnotherString));
            ArrangeAndAct();
            var actual = Result.Constants.Single(c => c.Name == SomeConstant).Value;
            Assert.Equal(AnotherString, actual);
        }
    }

    public class GivenBothHasEndpoint : WhenMerge
    {
        [Fact]
        public void ThenGetEndpointFromOther()
        {
            Step.Endpoint = "somewhere";
            Other.Endpoint = "elsewhere";
            ArrangeAndAct();
            Assert.Equal(Other.Endpoint, Result.Endpoint);
        }
    }

    public class GivenStepEndpointIsNull : WhenMerge
    {
        [Fact]
        public void ThenUseEndpointFromOther()
        {
            Other.Endpoint = "elsewhere";
            ArrangeAndAct();
            Assert.Equal(Other.Endpoint, Result.Endpoint);
        }
    }

    public class GivenBothHasStatusCodes : WhenMerge
    {
        [Fact]
        public void ThenGetStatusCodesFromStep()
        {
            Step.ExpectedStatusCodes = new[] { HttpStatusCode.Accepted };
            Other.ExpectedStatusCodes = new[] { HttpStatusCode.NotFound };
            ArrangeAndAct();
            Assert.Equal(Step.ExpectedStatusCodes, Result.ExpectedStatusCodes);
        }
    }

    public class GivenOnlyOtherHasResponse : WhenMerge
    {
        [Fact]
        public void ThenGetResponseFromOther()
        {
            Other.Response = new { A = "B" };
            ArrangeAndAct();
            Assert.Same(Other.Response, Result.Response);
        }
    }

    public class GivenBothHasResponse : WhenMerge
    {
        [Fact]
        public void ThenGetResponseFromOther()
        {
            Step.Response = new { B = "C" };
            Other.Response = new { A = "B" };
            ArrangeAndAct();
            Assert.Same(Other.Response, Result.Response);
        }
    }

    public class GivenBothHasBreakOnSuccess : WhenMerge
    {
        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void ThenGetBreakOnSuccessFromOther(bool inOther, bool inStep)
        {
            Other.BreakOnSuccess = inOther;
            Step.BreakOnSuccess = inStep;
            ArrangeAndAct();
            Assert.Equal(inOther, Result.BreakOnSuccess);
        }
    }

    public class GivenBothHasRetryOnFail : WhenMerge
    {
        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void ThenGetRetryOnFailFromOther(bool inStep, bool inOther)
        {
            Step.RetryOnFail = inStep;
            Other.RetryOnFail = inOther;
            ArrangeAndAct();
            Assert.Equal(inOther, Result.RetryOnFail);
        }
    }

    public class GivenBothHasDelayMs : WhenMerge
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        [InlineData(2, 2)]
        public void ThenGetDelayMsFromOther(int inStep, int inOther)
        {
            Step.DelayMs = inStep;
            Other.DelayMs = inOther;
            ArrangeAndAct();
            Assert.Equal(inOther, Result.DelayMs);
        }
    }

    public class GivenBothHasTimes : WhenMerge
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        [InlineData(2, 2)]
        public void ThenGetTimesFromOther(int inStep, int inOther)
        {
            Step.Times = inStep;
            Other.Times = inOther;
            ArrangeAndAct();
            Assert.Equal(inOther, Result.Times);
        }
    }

    public class GivenBothHaveType : WhenMerge
    {
        [Theory]
        [InlineData(StepType.Blob, StepType.Blob)]
        [InlineData(StepType.Blob, StepType.Rest)]
        [InlineData(StepType.Rest, StepType.Blob)]
        [InlineData(StepType.Rest, StepType.Rest)]
        public void ThenGetTypeFromOther(StepType inStep, StepType inOther)
        {
            Step.Type = inStep;
            Other.Type = inOther;
            ArrangeAndAct();
            Assert.Equal(inOther, Result.Type);
        }
    }

    protected static Constant[] CreateConstants(params (string name, string value)[] namedValues)
        => namedValues.Select(nv => new Constant { Name = nv.name, Value = nv.value }).ToArray();
}