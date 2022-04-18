// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages.Contracts;
using System.Threading.Tasks;

namespace PetImagesTest.Clients
{
    public interface IServiceClient
    {
        public Task<ServiceResponse<Account>> CreateAccountAsync(Account account);

        public Task<ServiceResponse<Image>> CreateOrUpdateImageAsync(string accountName, Image image);

        public Task<ServiceResponse<Image>> GetImageAsync(string accountName, string imageName);

        public Task<ServiceResponse<byte[]>> GetImageContentAsync(string accountName, string imageName);
    }
}
