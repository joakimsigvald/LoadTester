namespace Applique.LoadTester.Logic.Runtime.External
{
    public interface IBlobRepositoryFactory
    {
        IBlobRepository Create(string connectionString, string containerName, string folderName);
    }
}