using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Assembly;

namespace Applique.LoadTester.Domain.Service
{
    public interface IStepVerifierFactory
    {
        IStepVerifier CreateVerifier(IStep step, IBindings bindings);
    }
}