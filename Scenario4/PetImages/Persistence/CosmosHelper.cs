// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages.CosmosContracts;
using PetImages.Exceptions;
using System.Threading.Tasks;

namespace PetImages.Persistence
{
    public static class CosmosHelper
    {
        public static async Task<bool> DoesItemExistAsync<T>(ICosmosDatabase database, string containerName, string partitionKey, string id)
            where T : DbItem
        {
            try
            {
                await database.GetItemAsync<T>(containerName, partitionKey, id);
                return true;
            }
            catch (DatabaseItemDoesNotExistException)
            {
                return false;
            }
        }

        public static async Task<T> GetItemIfExistsAsync<T>(ICosmosDatabase database, string containerName, string partitionKey, string id)
            where T : DbItem
        {
            try
            {
                return await database.GetItemAsync<T>(containerName, partitionKey, id);
            }
            catch (DatabaseItemDoesNotExistException)
            {
                return null;
            }
        }
    }
}
