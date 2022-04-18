// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages.Exceptions;
using System.Threading.Tasks;

namespace PetImages.Persistence
{
    public static class StorageHelper
    {
        public static async Task CreateContainerIfNotExistsAsync(IStorageAccount storageAccount, string containerName)
        {
            try
            {
                await storageAccount.CreateContainerAsync(containerName);
            }
            catch (StorageContainerAlreadyExistsException)
            {
            }
        }

        public static async Task DeleteContainerIfExistsAsync(IStorageAccount storageAccount, string containerName)
        {
            try
            {
                await storageAccount.DeleteContainerAsync(containerName);
            }
            catch (StorageContainerDoesNotExistException)
            {
            }
        }

        public static async Task<byte[]> GetBlobIfExistsAsync(IStorageAccount storageAccount, string containerName, string blobName)
        {
            try
            {
                return await storageAccount.GetBlockBlobAsync(containerName, blobName);
            }
            catch (StorageContainerDoesNotExistException)
            {
                return null;
            }
            catch (BlobDoesNotExistException)
            {
                return null;
            }
        }

        public static async Task DeleteBlobIfExistsAsync(IStorageAccount storageAccount, string containerName, string blobName)
        {
            try
            {
                await storageAccount.DeleteBlockBlobAsync(containerName, blobName);
            }
            catch (StorageContainerDoesNotExistException)
            {
            }
            catch (BlobDoesNotExistException)
            {
            }
        }
    }
}
