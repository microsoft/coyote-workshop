// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages.Persistence;
using System;
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
            throw new NotImplementedException();
        }

        public Task DeleteContainerAsync(string containerName)
        {
            throw new NotImplementedException();
        }

        public Task CreateOrUpdateBlockBlobAsync(string containerName, string blobName, string contentType, byte[] blobContents)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetBlockBlobAsync(string containerName, string blobName)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBlockBlobAsync(string containerName, string blobName)
        {
            throw new NotImplementedException();
        }
    }
}
