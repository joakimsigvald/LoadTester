namespace Applique.LoadTester.Core.Result;

public interface IStepResult : ITimedResult
{
    string Endpoint { get; }
}