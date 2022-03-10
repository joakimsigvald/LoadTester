using Applique.LoadTester.Core.Design;

namespace Applique.LoadTester.Domain.Service;

public interface ILoader
{
    Constant[] LoadConstants<T>(string filename);
}