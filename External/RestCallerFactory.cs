using Applique.LoadTester.Business.Design;
using Applique.LoadTester.Business.Runtime;

namespace Applique.LoadTester.External
{
    public class RestCallerFactory : IRestCallerFactory
    {
        public IRestCaller Create(Service service, Endpoint endpoint, Bindings bindings)
            => new RestCaller(service, endpoint, bindings);
    }
}