using Applique.LoadTester.Domain.Assembly;

namespace Applique.LoadTester.Domain.Service
{
    public interface IBindingsRepositoryFactory
    {
        IBindingsRepository Create(ITestSuite testSuite);
    }
}