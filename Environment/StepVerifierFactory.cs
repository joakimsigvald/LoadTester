using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain;

namespace Applique.LoadTester.Environment
{
    public class StepVerifierFactory : IStepVerifierFactory 
    {
        public IStepVerifier CreateVerifier(Step step, IBindings bindings)
            => new StepVerifier(step, bindings);
    }
}