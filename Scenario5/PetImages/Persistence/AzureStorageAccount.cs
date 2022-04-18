// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using PetImages.Exceptions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PetImages.Persistence
{
    public class AzureStorageAccount : IStorageAccount
    {
        private readonly BlobServiceClient blobServiceClient;

        public AzureStorageAccount()
        {
            // Ref: https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator
            // Directly working with emulator, should come from configuration
            this.blobServiceClient = new BlobServiceClient("UseDevelopmentStorage=true;");
        }

        public async Task CreateContainerAsync(string containerName)
        {
            await PerformStorageOperationOrThrowAsync(async () =>
            {
                var containerClient = this.blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync();
            });
        }

        public async Task CreateOrUpdateBlockBlobAsync(string containerName, string blobName, string contentType, byte[] blobContents)
        {
            await PerformStorageOperationOrThrowAsync(async () =>
            {
                var containerClient = this.blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);
                using (var stream = new MemoryStream(blobContents))
                {
                    await blobClient.UploadAsync(
                        stream,
                        new BlobUploadOptions()
                        {
                            HttpHeaders = new BlobHttpHeaders()
                            {
                                ContentType = contentType
                            }
                        });
                }
            });
        }

        public async Task DeleteBlockBlobAsync(string containerName, string blobName)
        {
            await PerformStorageOperationOrThrowAsync(async () =>
            {
                var containerClient = this.blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.DeleteAsync();
            });
        }

        public async Task DeleteContainerAsync(string containerName)
        {
            await PerformStorageOperationOrThrowAsync(async () =>
            {
                var containerClient = this.blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.DeleteAsync();
            });
        }

        public async Task<byte[]> GetBlockBlobAsync(string containerName, string blobName)
        {
            byte[] result = null;
            await PerformStorageOperationOrThrowAsync(async () =>
            {
                var containerClient = this.blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);
                using (var blobStream = await blobClient.OpenReadAsync())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        blobStream.CopyTo(memoryStream);
                        result = memoryStream.ToArray();
                    }
                }
            });

            return result;
        }

        private async Task PerformStorageOperationOrThrowAsync(Func<Task> storageFunc)
        {
            try
            {
                await storageFunc();
            }
            catch (RequestFailedException requestFailedException)
            {
                throw StorageToDatabaseExceptionProvider(requestFailedException)();
            }
        }

        private Func<StorageException> StorageToDatabaseExceptionProvider(RequestFailedException requestFailedException)
        {
            if (requestFailedException.ErrorCode == "BlobAlreadyExists")
            {
                return () => new BlobAlreadyExistsException(requestFailedException);
            }
            else if (requestFailedException.ErrorCode == "BlobNotFound")
            {
                return () => new BlobDoesNotExistException(requestFailedException);
            }
            else if (requestFailedException.ErrorCode == "ContainerAlreadyExists")
            {
                return () => new StorageContainerAlreadyExistsException(requestFailedException);
            }
            else if (requestFailedException.ErrorCode == "ContainerNotFound")
            {
                return () => new StorageContainerDoesNotExistException(requestFailedException);
            }
            else
            {
                return () => new StorageException(requestFailedException);
            }
        }
    }
}
