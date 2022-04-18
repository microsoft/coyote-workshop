// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Storage.Queues;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace PetImages.Messaging
{
    public class StorageMessagingClient : IMessagingClient
    {
        private readonly QueueClient queueClient;

        public StorageMessagingClient(string queueName)
        {
            var developmentStoreConnectionString = "UseDevelopmentStorage=true;";
            this.queueClient = new QueueClient(developmentStoreConnectionString, queueName);
            queueClient.CreateIfNotExists();
        }

        public async Task SubmitMessage(Message message)
        {
            await queueClient.SendMessageAsync(JsonConvert.SerializeObject(message));
        }
    }
}
