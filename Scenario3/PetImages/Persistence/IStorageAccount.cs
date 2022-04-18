// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace PetImages.Persistence
{
    /// <summary>
    /// Interface of an Azure Storage blob container provider. This can be implemented
    /// for production or with a mock for (systematic) testing.
    /// </summary>
    public interface IStorageAccount
    {
        Task CreateContainerAsync(string containerName);

        Task DeleteContainerAsync(string containerName);

        Task CreateOrUpdateBlockBlobAsync(string containerName, string blobName, string contentType, byte[] blobContents);

        Task<byte[]> GetBlockBlobAsync(string containerName, string blobName);

        Task DeleteBlockBlobAsync(string containerName, string blobName);
    }
}
