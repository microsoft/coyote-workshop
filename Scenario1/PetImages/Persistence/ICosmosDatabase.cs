// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages.CosmosContracts;
using System.Threading.Tasks;

namespace PetImages.Persistence
{
    /// <summary>
    /// Interface of a Cosmos DB database. This can be implemented
    /// for production or with a mock for (systematic) testing.
    /// </summary>
    public interface ICosmosDatabase
    {
        Task CreateContainerIfNotExistsAsync(string containerName);

        public Task<T> CreateItemAsync<T>(string containerName, T row)
            where T : DbItem;

        public Task<T> GetItemAsync<T>(string containerName, string partitionKey, string id)
           where T : DbItem;

        public Task<T> UpsertItemAsync<T>(string containerName, T row, string ifMatchEtag = null)
            where T : DbItem;

        public Task<T> ReplaceItemAsync<T>(string containerName, T row, string ifMatchEtag = null)
            where T : DbItem;

        public Task DeleteItemAsync(string containerName, string partitionKey, string id, string ifMatchEtag = null);
    }
}
