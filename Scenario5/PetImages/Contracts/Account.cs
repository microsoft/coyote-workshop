// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages.CosmosContracts;

namespace PetImages.Contracts
{
    public class Account
    {
        public string Name { get; set; }

        public string ContactEmailAddress { get; set; }

        public AccountItem ToItem()
        {
            return new AccountItem()
            {
                Id = Name,
                ContactEmailAddress = ContactEmailAddress
            };
        }
    }
}
