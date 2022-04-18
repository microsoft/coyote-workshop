// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PetImages;
using PetImages.Persistence;
using PetImagesTest.PersistenceMocks;
using System.IO;
using System.Threading.Tasks;

namespace PetImagesTest
{
    internal class ServiceFactory : WebApplicationFactory<Startup>
    {
        private readonly ICosmosDatabase CosmosDatabase;

        public ServiceFactory()
        {
            this.CosmosDatabase = new MockCosmosDatabase(new MockCosmosState());
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
            });
        }
    }
}
