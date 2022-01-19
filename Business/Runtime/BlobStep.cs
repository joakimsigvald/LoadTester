using Applique.LoadTester.Business.Design;
using System;
using System.Threading.Tasks;

namespace Applique.LoadTester.Business.Runtime
{
    public class BlobStep : RunnableStep<object>
    {
        private readonly IBlobRepositoryFactory _factory;
        private readonly Blob _blob;
        private readonly Bindings _bindings;

        public static IRunnableStep Create(
            IBlobRepositoryFactory blobFactory, 
            TestSuite suite, 
            Step step, 
            Bindings bindings)
        {
            var blob = suite.GetBlob(step.Endpoint);
            return new BlobStep(blobFactory, step, blob, bindings);
        }

        private BlobStep(IBlobRepositoryFactory factory, Step step, Blob blob, Bindings bindings)
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
            await Task.Delay(Blueprint.Delay);
            Console.WriteLine($"Uploading to {Blueprint.Endpoint}");
            await repo.Upload(_bindings.SubstituteVariables(_blob.BlobName), content);
            return null;
        }
    }
}