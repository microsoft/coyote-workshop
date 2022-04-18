// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace PetImages.Messaging
{
    using Polly;
    using System.Threading.Tasks;

    public class WrappedMessagingClient : IMessagingClient
    {
        private readonly IAsyncPolicy AsyncPolicy;

        private readonly IMessagingClient MessagingClient;

        public WrappedMessagingClient(IMessagingClient messagingClient, IAsyncPolicy asyncPolicy)
        {
            this.MessagingClient = messagingClient;
            this.AsyncPolicy = asyncPolicy;
        }

        public Task SubmitMessage(Message message)
        {
            return this.AsyncPolicy.ExecuteAsync(() => this.MessagingClient.SubmitMessage(message));
        }
    }
}
