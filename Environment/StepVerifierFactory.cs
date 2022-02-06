using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Assembly;
using Applique.LoadTester.Domain.Service;

namespace Applique.LoadTester.Logic.Environment
{
    public class StepVerifierFactory : IStepVerifierFactory
    {
        public IStepVerifier CreateVerifier(IStep step, IBindings bindings)
            => new StepVerifier(step, bindings);
    }
}