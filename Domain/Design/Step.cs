using Applique.LoadTester.Core.Design;
using System;
using System.Net;

namespace Applique.LoadTester.Domain.Design
{
    public enum StepType { Rest, Blob }

    public class Step
    {
        public StepType Type { get; set; } = StepType.Rest;
        public string Template { get; set; }
        public Constant[] Constants { get; set; } = Array.Empty<Constant>();
        public string Endpoint { get; set; }
        public string Args { get; set; } = string.Empty;
        public HttpStatusCode[] ExpectedStatusCodes { get; set; } = new[] { HttpStatusCode.OK };
        public dynamic Body { get; set; }
        public dynamic Response { get; set; }
        public int DelayMs { get; set; } = 0;
        public int Times { get; set; } = 1;
        public bool BreakOnSuccess { get; set; }
        public bool RetryOnFail { get; set; }

        public Step MergeWith(Step other)
            => new()
            {
                Args = Args,
                Body = other.Body ?? Body,
                Constants = Constants.Merge(other.Constants),
                Endpoint = other.Endpoint ?? Endpoint,
                ExpectedStatusCodes = ExpectedStatusCodes,
                Response = other.Response ?? Response,
                BreakOnSuccess = BreakOnSuccess,
                RetryOnFail = RetryOnFail,
                DelayMs = DelayMs,
                Times = Times,
                Type = Type
            };
    }
}