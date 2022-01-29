namespace Applique.LoadTester.Core.Design
{
    public class Blob
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; } = string.Empty;
        public string ContainerName { get; set; }
        public string Folder { get; set; }
        public string BlobName { get; set; }
    }
}