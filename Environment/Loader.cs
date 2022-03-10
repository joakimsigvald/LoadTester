using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Service;

namespace Applique.LoadTester.Logic.Environment;

public class Loader : ILoader
{
    private readonly IFileSystem _fileSystem;

    public Loader(IFileSystem fileSystem) => _fileSystem = fileSystem;

    public Constant[] LoadConstants<T>(string filename) => _fileSystem.Read<Constant[]>(filename);
}