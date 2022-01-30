using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;

namespace Applique.LoadTester.Environment
{
    public class BindingsRepositoryFactory : IBindingsRepositoryFactory
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILoader _loader;
        private readonly IBindingsFactory _bindingsFactory;

        public BindingsRepositoryFactory(
            IFileSystem fileSystem,
            ILoader loader,
            IBindingsFactory bindingsFactory)
        {
            _fileSystem = fileSystem;
            _loader = loader;
            _bindingsFactory = bindingsFactory;
        }

        public IBindingsRepository Create(ITestSuite testSuite)
            => new BindingsRepository(_fileSystem, testSuite, _loader, _bindingsFactory);
    }
}