using Applique.LoadTester.Domain.Environment;
using Newtonsoft.Json;
using System.IO;

namespace Applique.LoadTester.External
{
    public class FileSystem : IFileSystem
    {
        private readonly string _basePath;

        public FileSystem(string basePath) => _basePath = basePath;

        public bool Exists(string fileName) => File.Exists(GetPath(fileName));

        public void Write(string fileName, object obj)
        {
            File.WriteAllText(GetPath(fileName), JsonConvert.SerializeObject(obj));
        }

        public void WriteLines(string fileName, string[] lines)
        {
            File.WriteAllLines(GetPath(fileName), lines);
        }

        public TValue Read<TValue>(string fileName)
        {
            var json = File.ReadAllText(GetPath(fileName));
            return JsonConvert.DeserializeObject<TValue>(json);
        }

        private string GetPath(string fileName) => Path.Combine(_basePath, fileName);
    }
}