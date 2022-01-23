using Applique.LoadTester.Runtime.Environment;
using Applique.LoadTester.Design;

namespace Applique.LoadTester.External
{
    public class RestCallerFactory : IRestCallerFactory
    {
        public IRestCaller Create(Service service, Endpoint endpoint, Bindings bindings)
            => new RestCaller(service, endpoint, bindings);
    }
}