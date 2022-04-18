// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages.CosmosContracts;
using PetImages.Exceptions;
using System;
using Container = System.Collections.Generic.Dictionary<string, PetImages.CosmosContracts.DbItem>;
using Database = System.Collections.Generic.Dictionary<
    string, System.Collections.Generic.Dictionary<string, PetImages.CosmosContracts.DbItem>>;

namespace PetImagesTest.PersistenceMocks
{
    public class MockCosmosState
    {
        private readonly Database Database = new Database();

        public void CreateContainer(string containerName)
        {
            EnsureContainerDoesNotExistInDatabase(containerName);
            _ = this.Database.TryAdd(containerName, new Container());
        }

        public void GetContainer(string containerName)
        {
            EnsureContainerExistsInDatabase(containerName);
        }

        public void DeleteContainer(string containerName)
        {
            EnsureContainerExistsInDatabase(containerName);
        }

        public void CreateItem(string containerName, DbItem item)
        {
            EnsureItemDoesNotExistInDatabase(containerName, item.PartitionKey, item.Id);

            item.ETag = Guid.NewGuid().ToString();

            var container = this.Database[containerName];
            container[GetCombinedKey(item.PartitionKey, item.Id)] = item;
        }

        public void UpsertItem(string containerName, DbItem item, string ifMatchEtag)
        {
            EnsureContainerExistsInDatabase(containerName);
            EnsureEtagMatch(containerName, item.PartitionKey, item.Id, ifMatchEtag);

            item.ETag = Guid.NewGuid().ToString();

            var container = this.Database[containerName];
            container[GetCombinedKey(item.PartitionKey, item.Id)] = item;
        }

        public void ReplaceItem(string containerName, DbItem item, string ifMatchEtag)
        {
            EnsureItemExistsInDatabase(containerName, item.PartitionKey, item.Id);
            EnsureEtagMatch(containerName, item.PartitionKey, item.Id, ifMatchEtag);

            item.ETag = Guid.NewGuid().ToString();

            var container = this.Database[containerName];
            container[GetCombinedKey(item.PartitionKey, item.Id)] = item;
        }

        public DbItem GetItem(string containerName, string partitionKey, string id)
        {
            EnsureItemExistsInDatabase(containerName, partitionKey, id);

            var container = this.Database[containerName];
            return container[GetCombinedKey(partitionKey, id)];
        }

        public void DeleteItem(string containerName, string partitionKey, string id, string ifMatchEtag)
        {
            EnsureItemExistsInDatabase(containerName, partitionKey, id);
            EnsureEtagMatch(containerName, partitionKey, id, ifMatchEtag);

            var container = this.Database[containerName];
            container.Remove(GetCombinedKey(partitionKey, id));
        }

        internal void EnsureContainerDoesNotExistInDatabase(string containerName)
        {
            if (this.Database.ContainsKey(containerName))
            {
                throw new DatabaseContainerAlreadyExistsException(cosmosException: null);
            }
        }

        internal void EnsureContainerExistsInDatabase(string containerName)
        {
            if (!this.Database.ContainsKey(containerName))
            {
                throw new DatabaseContainerDoesNotExistException(cosmosException: null);
            }
        }

        internal void EnsureItemExistsInDatabase(string containerName, string partitionKey, string id)
        {
            EnsureContainerExistsInDatabase(containerName);
            var container = this.Database[containerName];

            if (!container.ContainsKey(GetCombinedKey(partitionKey, id)))
            {
                throw new DatabaseItemDoesNotExistException(cosmosException: null);
            }
        }

        internal void EnsureItemDoesNotExistInDatabase(string containerName, string partitionKey, string id)
        {
            EnsureContainerExistsInDatabase(containerName);
            var container = this.Database[containerName];

            if (container.ContainsKey(GetCombinedKey(partitionKey, id)))
            {
                throw new DatabaseItemAlreadyExistsException(cosmosException: null);
            }
        }

        internal void EnsureEtagMatch(string containerName, string partitionKey, string id, string ifMatchEtag)
        {
            if (ifMatchEtag == null)
            {
                return;
            }

            var container = this.Database[containerName];

            var combinedKey = GetCombinedKey(partitionKey, id);
            if (container.ContainsKey(combinedKey))
            {
                var item = container[combinedKey];
                if (item.ETag != ifMatchEtag)
                {
                    throw new DatabasePreconditionFailedException(cosmosException: null);
                }
            }
        }

        internal static string GetCombinedKey(string partitionKey, string id)
        {
            return partitionKey + "_" + id;
        }
    }
}
