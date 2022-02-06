using System;
using System.Linq;
using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Assembly;
using Applique.LoadTester.Domain.Service;

namespace Applique.LoadTester.Logic.Environment
{
    public class BindingsRepository : IBindingsRepository
    {
        private readonly IFileSystem _fileSystem;
        private readonly ITestSuite _testSuite;
        private readonly ILoader _loader;
        private readonly IBindingsFactory _bindingsFactory;

        public BindingsRepository(
            IFileSystem fileSystem,
            ITestSuite testSuite,
            ILoader loader,
            IBindingsFactory bindingsFactory)
        {
            _fileSystem = fileSystem;
            _testSuite = testSuite;
            _loader = loader;
            _bindingsFactory = bindingsFactory;
        }

        public void PersistBindings(IBindings bindings, string[] propertiesToPersist)
            => _fileSystem.Write(BindingsPath, bindings
                .Join(propertiesToPersist, b => b.Name, p => p, (b, _) => b)
                .ToArray());

        public IBindings LoadBindings(string[] loadProperties)
        {
            if (!_fileSystem.Exists(BindingsPath) || !loadProperties.Any())
                return _bindingsFactory.CreateBindings(_testSuite, Array.Empty<Constant>());
            var constants = _loader.LoadConstants<Constant[]>(BindingsPath);
            var constantsToLoad = constants.Join(loadProperties, b => b.Name, p => p, (b, _) => b).ToArray();
            return _bindingsFactory.CreateBindings(_testSuite, constantsToLoad);
        }

        private string BindingsPath => $"{_testSuite.Name}_Bindings";
    }
}