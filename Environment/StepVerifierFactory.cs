using Applique.LoadTester.Domain.Design;
using Applique.LoadTester.Domain.Environment;

namespace Applique.LoadTester.Environment
{
    public class StepVerifierFactory : IStepVerifierFactory 
    {
        public IStepVerifier CreateVerifier(Step step, IBindings bindings)
            => new StepVerifier(step, bindings);
    }
}