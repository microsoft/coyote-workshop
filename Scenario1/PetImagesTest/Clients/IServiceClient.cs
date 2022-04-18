// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages.Contracts;
using System.Threading.Tasks;

namespace PetImagesTest.Clients
{
    public interface IServiceClient
    {
        public Task<ServiceResponse<Account>> CreateAccountAsync(Account account);
    }
}
