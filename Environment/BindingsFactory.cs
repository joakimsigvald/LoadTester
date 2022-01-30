﻿using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Service;
using System.Collections.Generic;
using System.Linq;

namespace Applique.LoadTester.Environment
{
    public class BindingsFactory : IBindingsFactory
    {
        private readonly IFileSystem _fileSystem;

        public BindingsFactory(IFileSystem fileSystem) => _fileSystem = fileSystem;

        public IBindings CreateInstanceBindings(ITestSuite testSuite, IScenario scenario, int instanceId)
        {
            var constants = GetConstants(testSuite, scenario.Constants, instanceId);
            return CreateBindings(testSuite, constants, testSuite.Models);
        }

        public IBindings CreateBindings(ITestSuite testSuite, Constant[] constants, Model[] models = null)
            => new Bindings(new BindingVariables(CreateVariables(_fileSystem, testSuite, constants, models)));

        private static Constant[] GetConstants(ITestSuite testSuite, Constant[] scenarioConstants, int instanceId)
            => ConstantFactory.Merge(GetInstanceConstants(testSuite, instanceId), scenarioConstants);

        private static IEnumerable<Constant> GetInstanceConstants(ITestSuite testSuite, int instanceId)
            => testSuite.Constants.Prepend(ConstantFactory.Create("InstanceId", $"{instanceId}"));

        private static IDictionary<string, object> CreateVariables(
            IFileSystem fileSystem,
            ITestSuite suite,
            IEnumerable<Constant> constants,
            Model[] models = null)
        {
            var valueRetriever = new ValueRetriever(fileSystem, suite);
            var variables = constants.ToDictionary(c => c.Name, valueRetriever.GetValue);
            if (models?.Any() == true)
                foreach (var model in models)
                    variables[model.Name] = model.Value;
            return variables;
        }
    }
}