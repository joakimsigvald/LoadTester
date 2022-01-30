using Applique.LoadTester.Domain.Design;

namespace Applique.LoadTester.Domain.Service
{
    public interface IBindingsRepositoryFactory
    {
        IBindingsRepository Create(ITestSuite testSuite);
    }
}