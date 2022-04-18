// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages;
using PetImages.Exceptions;
using PetImages.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetImagesTest.PersistenceMocks
{
    internal class MockStorageAccount : IStorageAccount
    {
        private readonly Dictionary<string, Dictionary<string, byte[]>> Containers;

        internal MockStorageAccount()
        {
            this.Containers = new Dictionary<string, Dictionary<string, byte[]>>();
        }

        public Task CreateContainerAsync(string containerName)
        {
            return Task.Run(() =>
            {
                Logger.WriteLine($"Attempting to create storage container {containerName}");

                EnsureContainerDoesNotExist(containerName);
                Containers[containerName] = new Dictionary<string, byte[]>();
            });
        }

        public Task DeleteContainerAsync(string containerName)
        {
            return Task.Run(() =>
            {
                Logger.WriteLine($"Attempting to delete storage container {containerName}");

                EnsureContainerExists(containerName);
                Containers.Remove(containerName);
            });
        }

        public Task CreateOrUpdateBlockBlobAsync(string containerName, string blobName, string contentType, byte[] blobContents)
        {
            return Task.Run(() =>
            {
                Logger.WriteLine($"Attempting to create or update block blob: container {containerName}, blob {blobName}");

                EnsureContainerExists(containerName);
                var container = Containers[containerName];
                container[blobName] = blobContents;
            });
        }

        public Task<byte[]> GetBlockBlobAsync(string containerName, string blobName)
        {
            return Task.Run(() =>
            {
                Logger.WriteLine($"Attempting to get block blob: container {containerName} blob {blobName}");

                EnsureBlobExists(containerName, blobName);
                var container = Containers[containerName];
                return container[blobName];
            });
        }

        public Task DeleteBlockBlobAsync(string containerName, string blobName)
        {
            return Task.Run(() =>
            {
                Logger.WriteLine($"Attempting to delete block blob: container {containerName} blob {blobName}");

                EnsureBlobExists(containerName, blobName);
                var container = Containers[containerName];
                container.Remove(blobName);
            });
        }

        private void EnsureContainerDoesNotExist(string containerName)
        {
            if (Containers.ContainsKey(containerName))
            {
                throw new StorageContainerAlreadyExistsException(requestFailedException: null);
            }
        }

        private void EnsureContainerExists(string containerName)
        {
            if (!Containers.ContainsKey(containerName))
            {
                throw new StorageContainerDoesNotExistException(requestFailedException: null);
            }
        }

        private void EnsureBlobExists(string containerName, string blobName)
        {
            EnsureContainerExists(containerName);

            var container = Containers[containerName];

            if (!container.ContainsKey(blobName))
            {
                throw new BlobDoesNotExistException(requestFailedException: null);
            }
        }
    }
}
