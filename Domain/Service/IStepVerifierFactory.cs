using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;

namespace Applique.LoadTester.Domain.Service
{
    public interface IStepVerifierFactory
    {
        IStepVerifier CreateVerifier(Step step, IBindings bindings);
    }
}