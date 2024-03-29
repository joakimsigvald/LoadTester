﻿using Applique.LoadTester.Logic.Runtime.External;
using Azure.Storage.Blobs;

namespace Applique.LoadTester.External;

public class BlobRepositoryFactory : IBlobRepositoryFactory
{
    public IBlobRepository Create(
        string connectionString, string containerName, string folderName)
    {
        var container = CreateBlobContainerClient(connectionString, containerName);
        return new BlobRepository(container, folderName);
    }

    private static BlobContainerClient CreateBlobContainerClient(string connectionString, string containerName)
        => new(connectionString, containerName);
}