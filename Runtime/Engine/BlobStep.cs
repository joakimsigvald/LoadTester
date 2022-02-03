using System;
using System.Threading.Tasks;
using Applique.LoadTester.Runtime.External;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;

namespace Applique.LoadTester.Runtime.Engine
{
    public class BlobStep : RunnableStep<object>
    {
        private readonly IBlobRepositoryFactory _factory;
        private readonly Blob _blob;

        public static IRunnableStep Create(
            IBlobRepositoryFactory blobFactory,
            ITestSuite suite,
            Step step,
            IBindings bindings,
            IBindings overloads)
        {
            var blob = suite.GetBlob(step.Endpoint);
            return new BlobStep(blobFactory, step, blob, bindings, overloads);
        }

        private BlobStep(IBlobRepositoryFactory factory, Step step, Blob blob, IBindings bindings, IBindings overloads)
            : base(step, bindings, overloads)
        {
            _factory = factory;
            _blob = blob;
        }

        protected override async Task<object> DoRun()
        {
            var repo = _factory.Create(_blob.ConnectionString, _blob.ContainerName, _blob.Folder);
            var content = _bindings.CreateContent(Blueprint.Body);
            await Task.Delay(Delay);
            Console.WriteLine($"Uploading to {Blueprint.Endpoint}");
            await repo.Upload(_bindings.SubstituteVariables(_blob.BlobName), content);
            return null;
        }
    }
}