using System.IO;
using System.Threading.Tasks;
using Applique.LoadTester.Logic.Runtime.External;
using Azure.Storage.Blobs;

namespace Applique.LoadTester.External;

public class BlobRepository : IBlobRepository
{
    private readonly BlobContainerClient _container;
    private readonly string _folderName;

    public BlobRepository(BlobContainerClient container, string folderName)
    {
        _container = container;
        _folderName = folderName;
    }

    public Task Upload(string name, string text)
    {
        var path = $"{_folderName}/{name}";
        var blobClient = _container.GetBlobClient(path);
        var stream = GenerateStreamFromString(text);
        return blobClient.UploadAsync(stream);
    }

    private static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}