using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Environment;

namespace Applique.LoadTester.External
{
    public class RestCallerFactory : IRestCallerFactory
    {
        public IRestCaller Create(Service service, Endpoint endpoint, IBindings bindings)
            => new RestCaller(service, endpoint, bindings);
    }
}