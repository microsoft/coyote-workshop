// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Storage.Queues;
using Newtonsoft.Json;
using PetImages.Messaging;
using System.Threading.Tasks;

namespace PetImages.Worker
{
    public class StorageMessageReceiverClient : IMessageReceiver
    {
        private readonly QueueClient queueClient;

        public StorageMessageReceiverClient(string queueName)
        {
            var developmentStoreConnectionString = "UseDevelopmentStorage=true";
            this.queueClient = new QueueClient(developmentStoreConnectionString, queueName);
        }

        public async Task<Message> ReadMessage()
        {
            var messageResponse = await this.queueClient.ReceiveMessageAsync();

            if (messageResponse.Value == null)
            {
                return null;
            }

            // Also delete the message from the queue, reading doesnt delete it
            await this.queueClient.DeleteMessageAsync(messageResponse.Value.MessageId, messageResponse.Value.PopReceipt);

            // Hack for now, but should be fixed to work with Polymorphic De/Serialization
            var messageObject = JsonConvert.DeserializeObject<GenerateThumbnailMessage>(messageResponse.Value.MessageText);
            return messageObject;
        }
    }
}
