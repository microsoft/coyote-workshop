// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages;
using PetImages.CosmosContracts;
using PetImages.Exceptions;
using PetImages.Persistence;
using System.Threading.Tasks;

namespace PetImagesTest.PersistenceMocks
{
    public class MockCosmosDatabase : ICosmosDatabase
    {
        private readonly MockCosmosState State;

        public MockCosmosDatabase(MockCosmosState state)
        {
            this.State = state;
        }

        public Task CreateContainerIfNotExistsAsync(string containerName)
        {
            return Task.Run(() =>
            {
                Logger.WriteLine($"Createing CosmosDB container {containerName} if it does not exist");

                try
                {
                    this.State.CreateContainer(containerName);
                }
                catch (DatabaseContainerAlreadyExistsException)
                {
                }
            });
        }

        public Task<T> CreateItemAsync<T>(string containerName, T item)
            where T : DbItem
        {
            var itemCopy = TestHelper.Clone(item);

            return Task.Run(() =>
            {
                Logger.WriteLine($"Attempting to create an item with partition key: {item.PartitionKey}, id: {item.Id}");

                this.State.CreateItem(containerName, itemCopy);
                return itemCopy;
            });
        }

        public Task<T> GetItemAsync<T>(string containerName, string partitionKey, string id)
            where T : DbItem
        {
            return Task.Run(() =>
            {
                Logger.WriteLine($"Attempting to get an item with partition key: {partitionKey}, id: {id}");

                var item = this.State.GetItem(containerName, partitionKey, id);

                var itemCopy = TestHelper.Clone((T)item);

                return itemCopy;
            });
        }

        public Task<T> UpsertItemAsync<T>(string containerName, T item, string ifMatchEtag = null)
            where T : DbItem
        {
            return Task.Run(() =>
            {
                Logger.WriteLine($"Attempting to upsert an item with partition key: {item.PartitionKey}, id: {item.Id}");

                var itemCopy = TestHelper.Clone(item);
                this.State.UpsertItem(containerName, itemCopy, ifMatchEtag);
                return itemCopy;
            });
        }

        public Task<T> ReplaceItemAsync<T>(string containerName, T item, string ifMatchEtag = null)
            where T : DbItem
        {
            return Task.Run(() =>
            {
                Logger.WriteLine($"Attempting to replace an item with partition key: {item.PartitionKey}, id: {item.Id}");

                var itemCopy = TestHelper.Clone(item);
                this.State.ReplaceItem(containerName, itemCopy, ifMatchEtag);
                return itemCopy;
            });
        }

        public Task DeleteItemAsync(string containerName, string partitionKey, string id, string ifMatchEtag = null)
        {
            return Task.Run(() =>
            {
                Logger.WriteLine($"Attempting to delete an item with partition key: {partitionKey}, id: {id}");

                this.State.DeleteItem(containerName, partitionKey, id, ifMatchEtag);
            });
        }
    }
}
