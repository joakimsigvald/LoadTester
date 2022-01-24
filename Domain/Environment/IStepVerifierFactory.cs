using Applique.LoadTester.Domain.Design;

namespace Applique.LoadTester.Domain.Environment
{
    public interface IStepVerifierFactory
    {
        IStepVerifier CreateVerifier(Step step, IBindings bindings);
    }
}