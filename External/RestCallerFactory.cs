using Applique.LoadTester.Runtime.External;

namespace Applique.LoadTester.External
{
    public class RestCallerFactory : IRestCallerFactory
    {
        public IRestCaller Create(string baseUrl) 
            => new RestCaller(baseUrl);
    }
}