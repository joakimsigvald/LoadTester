using Applique.LoadTester.Core.Service;

namespace Applique.LoadTester.Domain.Service;

public interface IBindingsRepository
{
    void PersistBindings(IBindings bindings, string[] propertiesToPersist);
    IBindings LoadBindings(string[] loadProperties);
}