using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Domain.Assembly;
using Applique.LoadTester.Domain.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Applique.LoadTester.Logic.Assembly;

public class Step : IStep
{
    public StepType Type { get; set; } = StepType.Rest;
    public string Template { get; set; }
    public Constant[] Constants { get; set; } = Array.Empty<Constant>();
    public string Endpoint { get; set; }
    public string Args { get; set; } = string.Empty;
    public HttpStatusCode[] ExpectedStatusCodes { get; set; } = new[] { HttpStatusCode.OK };
    public dynamic Body { get; set; }
    public dynamic Response { get; set; }
    public int DelayMs { get; set; }
    public int Times { get; set; } = 1;
    public bool BreakOnSuccess { get; set; }
    public bool RetryOnFail { get; set; }

    public IStep MergeWith(IStep other)
        => new Step()
        {
            Args = MergeArgs(Args, other.Args),
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

    private static string MergeArgs(string args1, string args2)
    {
        var argDict1 = ExtractQuery(args1);
        foreach (var kvp in ExtractQuery(args2))
            argDict1[kvp.Key] = kvp.Value;
        return argDict1.Any()
            ? string.Join('&', argDict1.Select(kvp => $"{kvp.Key}={kvp.Value}"))
            : string.Empty;
    }

    private static IDictionary<string, string> ExtractQuery(string args)
        => args
        .Split('&', StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Split('='))
        .GroupBy(arr => arr[0])
        .ToDictionary(g => g.Key, g => string.Join(',', g.Select(arr => arr[1])));
}