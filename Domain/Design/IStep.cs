using Applique.LoadTester.Core.Design;
using System.Net;

namespace Applique.LoadTester.Domain.Design
{
    public enum StepType { Rest, Blob }

    public interface IStep
    {
        StepType Type { get; }
        string Template { get; }
        Constant[] Constants { get; }
        string Endpoint { get; }
        string Args { get; }
        dynamic Body { get; }
        dynamic Response { get; }
        int DelayMs { get; }
        IStep MergeWith(IStep other);
        HttpStatusCode[] ExpectedStatusCodes { get; }
        int Times { get; }
        bool BreakOnSuccess { get; }
        bool RetryOnFail { get; }
    }
}