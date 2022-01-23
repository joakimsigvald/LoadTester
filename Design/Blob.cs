namespace Applique.LoadTester.Design
{
    public class Blob
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; } = string.Empty;
        public string ContainerName { get; set; } = "Get";
        public string Folder { get; set; } = "Get";
        public string BlobName { get; set; } = "Get";
    }
}