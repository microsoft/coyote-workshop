// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using PetImages.CosmosContracts;
using PetImages.Exceptions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PetImages.Persistence
{
    public class CosmosDatabase : ICosmosDatabase
    {
        private readonly CosmosClient cosmosClient;

        private Database cosmosDatabase;

        private readonly string databaseName;

        public static ICosmosDatabase CreateDatabaseIfNotExists(string databaseName)
        {
            var instance = new CosmosDatabase(databaseName);
            instance.Initialize();
            return instance;
        }

        public CosmosDatabase(string databaseName)
        {
            // Connect to the Azure Cosmos DB Emulator running locally
            // Ideally the endpoint and key should come from config
            // Ensure Emulator v2.14.6, otherwise initialization will fail
            this.cosmosClient = new CosmosClient(
               "https://localhost:8081",
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            this.databaseName = databaseName;
        }

        public async Task CreateContainerIfNotExistsAsync(string containerName)
        {
            await this.cosmosDatabase.CreateContainerIfNotExistsAsync(
                new ContainerProperties
                {
                    Id = containerName,
                    PartitionKeyPath = "/partitionKey",
                });
        }

        public async Task<T> CreateItemAsync<T>(string containerName, T row) where T : DbItem
        {
            return await this.PerformCosmosOperationOrThrowAsync<T>(
                () => cosmosClient.GetContainer(databaseName, containerName).CreateItemAsync(row));
        }

        public async Task DeleteItemAsync(string containerName, string partitionKey, string id, string ifMatchEtag = null)
        {
            _ = await this.PerformCosmosOperationOrThrowAsync<DbItem>(
                () => cosmosClient.GetContainer(databaseName, containerName).DeleteItemAsync<DbItem>(
                    id,
                    new PartitionKey(partitionKey),
                    new ItemRequestOptions() { IfMatchEtag = ifMatchEtag }));
        }

        public async Task<T> GetItemAsync<T>(string containerName, string partitionKey, string id) where T : DbItem
        {
            return await this.PerformCosmosOperationOrThrowAsync(() =>
                cosmosClient.GetContainer(databaseName, containerName).ReadItemAsync<T>(
                    id, new PartitionKey(partitionKey)));
        }

        public async Task<T> ReplaceItemAsync<T>(string containerName, T row, string ifMatchEtag = null) where T : DbItem
        {
            return await this.PerformCosmosOperationOrThrowAsync(() =>
                cosmosClient.GetContainer(databaseName, containerName).ReplaceItemAsync(
                    row,
                    row.Id,
                    new PartitionKey(row.PartitionKey),
                    new ItemRequestOptions() { IfMatchEtag = ifMatchEtag }));
        }

        public async Task<T> UpsertItemAsync<T>(string containerName, T row, string ifMatchEtag = null) where T : DbItem
        {
            return await this.PerformCosmosOperationOrThrowAsync(() =>
                cosmosClient.GetContainer(databaseName, containerName).UpsertItemAsync(
                    row,
                    new PartitionKey(row.PartitionKey),
                    new ItemRequestOptions() { IfMatchEtag = ifMatchEtag }));
        }

        private async Task<T> PerformCosmosOperationOrThrowAsync<T>(Func<Task<ItemResponse<T>>> cosmosFunc)
            where T : DbItem
        {
            try
            {
                var response = await cosmosFunc();
                return response.Resource;
            }
            catch (CosmosException cosmosException)
            {
                throw CosmosToDatabaseExceptionProvider(cosmosException)();
            }
        }

        private Func<DatabaseException> CosmosToDatabaseExceptionProvider(CosmosException cosmosException)
        {
            if (cosmosException.StatusCode == HttpStatusCode.NotFound)
            {
                return () => new DatabaseItemDoesNotExistException(cosmosException);
            }
            else if (cosmosException.StatusCode == HttpStatusCode.Conflict)
            {
                return () => new DatabaseItemAlreadyExistsException(cosmosException);
            }
            else if (cosmosException.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                return () => new DatabasePreconditionFailedException(cosmosException);
            }
            else
            {
                return () => new DatabaseException(cosmosException);
            }
        }

        private void Initialize()
        {
            _ = this.cosmosClient.CreateDatabaseIfNotExistsAsync(this.databaseName).Result;
            this.cosmosDatabase = this.cosmosClient.GetDatabase(this.databaseName);
        }
    }
}
