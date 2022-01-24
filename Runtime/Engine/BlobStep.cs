using System;
using System.Threading.Tasks;
using Applique.LoadTester.Domain.Environment;
using Applique.LoadTester.Domain.Design;

namespace Applique.LoadTester.Runtime.Engine
{
    public class BlobStep : RunnableStep<object>
    {
        private readonly IBlobRepositoryFactory _factory;
        private readonly Blob _blob;
        private readonly IBindings _bindings;

        public static IRunnableStep Create(
            IBlobRepositoryFactory blobFactory,
            ITestSuite suite,
            Step step,
            IBindings bindings)
        {
            var blob = suite.GetBlob(step.Endpoint);
            return new BlobStep(blobFactory, step, blob, bindings);
        }

        private BlobStep(IBlobRepositoryFactory factory, Step step, Blob blob, IBindings bindings)
            : base(step)
        {
            _factory = factory;
            _blob = blob;
            _bindings = bindings;
        }

        protected override Task HandleResponse(object _)
            => Task.CompletedTask;

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