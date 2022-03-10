namespace Applique.LoadTester.Logic.Runtime.External;

public interface IRestCallerFactory
{
    IRestCaller Create(string baseUrl);
}