using Applique.LoadTester.Design;

namespace Applique.LoadTester.Runtime.Environment
{
    public interface IRestCallerFactory
    {
        IRestCaller Create(Service service, Endpoint endpoint, Bindings bindings);
    }
}