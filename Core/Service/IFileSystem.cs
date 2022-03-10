namespace Applique.LoadTester.Core.Service;

public interface IFileSystem
{
    bool Exists(string fileName);

    void Write(string fileName, object obj);

    void WriteLines(string fileName, string[] lines);

    TValue Read<TValue>(string fileName);
}