namespace Applique.LoadTester.Runtime.Environment
{
    public interface IBlobRepositoryFactory
    {
        IBlobRepository Create(string connectionString, string containerName, string folderName);
    }
}