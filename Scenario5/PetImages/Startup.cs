// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Coyote.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PetImages.Messaging;
using PetImages.Middleware;
using PetImages.Persistence;

namespace PetImages
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "PetImages API",
                    Version = "v1",
                    Description = "Description for the API goes here.",
                });
            });

            // Add CosmosServices
            services.AddSingleton<ICosmosDatabase>(s =>
            {
                var database = CosmosDatabase.CreateDatabaseIfNotExists(Constants.DatabaseName);

                database.CreateContainerIfNotExistsAsync(Constants.AccountContainerName).Wait();
                database.CreateContainerIfNotExistsAsync(Constants.ImageContainerName).Wait();

                return database;
            });

            // Add BlobStorage Services
            services.AddSingleton<IStorageAccount>(s => new AzureStorageAccount());

            // Add Messaging Services
            services.AddSingleton<IMessagingClient>(s => new StorageMessagingClient(Constants.ThumbnailQueueName));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add Coyote Middleware that takes control of the controllers during testing.
            app.UseRequestController();

            app.UseMiddleware<RequestIdMiddleware>();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PetImages API");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
