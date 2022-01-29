namespace Applique.LoadTester.Runtime.External
{
    public interface IRestCallerFactory
    {
        IRestCaller Create(string baseUrl);
    }
}