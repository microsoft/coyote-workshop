// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages.CosmosContracts;
using Polly;
using System.Threading.Tasks;

namespace PetImages.Persistence
{
    public class WrappedCosmosDatabase : ICosmosDatabase
    {
        private readonly IAsyncPolicy AsyncPolicy;

        private readonly ICosmosDatabase CosmosDatabase;

        public WrappedCosmosDatabase(ICosmosDatabase cosmosDatabase, IAsyncPolicy asyncPolicy)
        {
            this.CosmosDatabase = cosmosDatabase;
            this.AsyncPolicy = asyncPolicy;
        }

        public Task CreateContainerIfNotExistsAsync(string containerName)
        {
            return this.AsyncPolicy.ExecuteAsync(() => this.CosmosDatabase.CreateContainerIfNotExistsAsync(containerName));
        }

        public Task<T> CreateItemAsync<T>(string containerName, T row) where T : DbItem
        {
            return this.AsyncPolicy.ExecuteAsync(() => this.CosmosDatabase.CreateItemAsync(containerName, row));
        }

        public Task DeleteItemAsync(string containerName, string partitionKey, string id, string ifMatchEtag = null)
        {
            return this.AsyncPolicy.ExecuteAsync(() => this.CosmosDatabase.DeleteItemAsync(containerName, partitionKey, id, ifMatchEtag));
        }

        public Task<T> GetItemAsync<T>(string containerName, string partitionKey, string id) where T : DbItem
        {
            return this.AsyncPolicy.ExecuteAsync(() => this.CosmosDatabase.GetItemAsync<T>(containerName, partitionKey, id));
        }

        public Task<T> ReplaceItemAsync<T>(string containerName, T row, string ifMatchEtag = null) where T : DbItem
        {
            return this.AsyncPolicy.ExecuteAsync(() => this.CosmosDatabase.ReplaceItemAsync(containerName, row, ifMatchEtag));
        }

        public Task<T> UpsertItemAsync<T>(string containerName, T row, string ifMatchEtag = null) where T : DbItem
        {
            return this.AsyncPolicy.ExecuteAsync(() => this.CosmosDatabase.UpsertItemAsync(containerName, row, ifMatchEtag));
        }
    }
}
