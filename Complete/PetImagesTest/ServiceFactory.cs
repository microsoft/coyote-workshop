// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PetImages;
using PetImages.Messaging;
using PetImages.RetryFramework;
using PetImages.Persistence;
using PetImagesTest.MessagingMocks;
using PetImagesTest.PersistenceMocks;
using System.IO;
using System.Threading.Tasks;

namespace PetImagesTest
{
    internal class ServiceFactory : WebApplicationFactory<Startup>
    {
        private readonly IStorageAccount StorageAccount;
        private readonly ICosmosDatabase CosmosDatabase;
        private readonly IMessagingClient MessagingClient;

        public ServiceFactory()
        {
            this.StorageAccount = new WrappedStorageAccount(
                new MockStorageAccount(),
                RetryPolicyFactory.GetAsyncRetryExponential());

            this.CosmosDatabase = new WrappedCosmosDatabase(
                new MockCosmosDatabase(new MockCosmosState()),
                RetryPolicyFactory.GetAsyncRetryExponential());

            var messagingClient = new MockMessagingClient(this.CosmosDatabase, this.StorageAccount);
            this.MessagingClient = new WrappedMessagingClient(
                messagingClient,
                RetryPolicyFactory.GetAsyncRetryExponential());
        }

        internal async Task InitializeCosmosDatabaseAsync()
        {
            await this.CosmosDatabase.CreateContainerIfNotExistsAsync(Constants.AccountContainerName);
            await this.CosmosDatabase.CreateContainerIfNotExistsAsync(Constants.ImageContainerName);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            builder.ConfigureServices(services =>
            {
                // Inject the mocks.
                services.AddSingleton(this.CosmosDatabase);
                services.AddSingleton(this.StorageAccount);
                services.AddSingleton(this.MessagingClient);
            });
        }
    }
}
