using Applique.LoadTester.Domain.Design;

namespace Applique.LoadTester.Domain.Environment
{
    public interface IRestCallerFactory
    {
        IRestCaller Create(Service service, Endpoint endpoint, IBindings bindings);
    }
}