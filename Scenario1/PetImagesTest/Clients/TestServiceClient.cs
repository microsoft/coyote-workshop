// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Mvc.Testing;
using PetImages.Contracts;
using PetImagesTest.Exceptions;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace PetImagesTest.Clients
{
    internal class TestServiceClient : IServiceClient, IDisposable
    {
        private readonly HttpClient Client;

        private static readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        internal TestServiceClient(ServiceFactory factory)
        {
            this.Client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false,
                HandleCookies = false
            });
        }

        public async Task<ServiceResponse<Account>> CreateAccountAsync(Account account)
        {
            var response = await this.Client.PostAsync(
                new Uri($"accounts", UriKind.RelativeOrAbsolute),
                JsonContent.Create(account));

            return await ConstructServiceResponseAsync<Account>(response);
        }

        public void Dispose()
        {
            this.Client.Dispose();
        }

        private static async Task<ServiceResponse<T>> ConstructServiceResponseAsync<T>(HttpResponseMessage httpResponse)
            where T : class
        {
            var statusCode = (int)httpResponse.StatusCode;

            if (statusCode >= 200 && statusCode <= 299)
            {
                return new ServiceResponse<T>()
                {
                    StatusCode = httpResponse.StatusCode,
                    Resource = JsonSerializer.Deserialize<T>(
                        await httpResponse.Content.ReadAsStringAsync(),
                        serializerOptions)
                };
            }
            else if (statusCode >= 400 && statusCode <= 499)
            {
                return new ServiceResponse<T>()
                {
                    StatusCode = httpResponse.StatusCode,
                    Error = JsonSerializer.Deserialize<Error>(
                        await httpResponse.Content.ReadAsStringAsync(),
                        serializerOptions)
                };
            }
            else if (statusCode >= 500 && statusCode <= 599)
            {
                throw new InternalServerErrorException(httpResponse);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
