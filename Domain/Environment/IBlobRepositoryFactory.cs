namespace Applique.LoadTester.Domain.Environment
{
    public interface IBlobRepositoryFactory
    {
        IBlobRepository Create(string connectionString, string containerName, string folderName);
    }
}