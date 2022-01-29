using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Service;

namespace Applique.LoadTester.Domain
{
    public interface IStepVerifierFactory
    {
        IStepVerifier CreateVerifier(Step step, IBindings bindings);
    }
}