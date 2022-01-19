namespace Applique.LoadTester.Business.Runtime
{
    public interface IBlobRepositoryFactory
    {
        IBlobRepository Create(string connectionString, string containerName, string folderName);
    }
}