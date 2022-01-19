using Applique.LoadTester.Business.Design;

namespace Applique.LoadTester.Business.Runtime
{
    public interface IRestCallerFactory
    {
        IRestCaller Create(Service service, Endpoint endpoint, Bindings bindings);
    }
}